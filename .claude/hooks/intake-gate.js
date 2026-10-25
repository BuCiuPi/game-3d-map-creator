#!/usr/bin/env node
/**
 * PreToolUse gate: refuse the first code write of a work session until a feature
 * intake has been recorded in docs/INTAKE_LOG.md.
 *
 * Why this exists: `docs/FEATURE_INTAKE.md` is imported into every session and was
 * still skipped on a large multi-file implementation. Prose in an always-loaded
 * file was not enough. This is. Markdown is the only source of truth, so the gate
 * reads the log directly: no binary, no database, just a tracked text file every
 * clone has.
 *
 * Design constraints, in priority order:
 *
 *  1. FAIL OPEN, ALWAYS. Any missing file, read failure, or parse failure allows the
 *     write. A gate that blocks the team is worse than the problem it solves. The
 *     only path that blocks is: the log exists, parses, and holds no recent entry.
 *  2. Guard code, not prose. Paths under `exemptRoots` (default `docs`, `.claude`)
 *     and every `.md` / `.txt` file are always allowed, so recording an intake and
 *     writing the story it demands are never themselves blocked. When
 *     `guardedRoots` is non-empty only paths under those roots are gated; when it
 *     is empty or missing, every other path in the repository is gated.
 *     `guardedDocRoots` (default none) reverses this for prose that IS the
 *     product, e.g. `docs/design` in a game-design repository: every file under
 *     those roots is gated, markdown included.
 *  3. Cheap to satisfy. The block message contains the exact row to add.
 *
 * Configuration: `<repoRoot>/.claude/harness.config.json`
 *   { "guardedRoots": [], "exemptRoots": ["docs", ".claude"], "intakeWindowHours": 24 }
 * A missing or unreadable config falls back to these defaults.
 *
 * Window semantics: "has an intake been recorded recently", not "for this exact
 * task" — a hook has no session identity. The comparison uses ABSOLUTE difference so
 * a log written in local time while the hook reads UTC cannot produce a false block.
 * INTAKE_GATE_WINDOW_HOURS overrides the window; INTAKE_GATE_OFF=1 disables.
 */
const { existsSync, readFileSync } = require('node:fs');
const { dirname, join, resolve, relative } = require('node:path');

const ALLOW = 0;
const BLOCK = 2;

const LOG_PATH = 'docs/INTAKE_LOG.md';
const CONFIG_PATH = '.claude/harness.config.json';
const DEFAULTS = { guardedRoots: [], guardedDocRoots: [], exemptRoots: ['docs', '.claude'], intakeWindowHours: 24 };

function allow() {
  process.exit(ALLOW);
}

try {
  main();
} catch {
  allow();
}

function main() {
  if (process.env.INTAKE_GATE_OFF === '1') allow();

  let event;
  try {
    event = JSON.parse(readFileSync(0, 'utf8'));
  } catch {
    allow();
  }

  const filePath = event?.tool_input?.file_path ?? event?.tool_input?.notebook_path;
  if (!filePath || typeof filePath !== 'string') allow();

  const cwd = event?.cwd || process.env.CLAUDE_PROJECT_DIR || process.cwd();
  const target = resolve(cwd, filePath);
  // The repo is the nearest folder holding the intake log: the session's cwd can be
  // a subfolder, and CLAUDE_PROJECT_DIR stays put when Claude enters a worktree.
  const repoRoot = findRepoRoot(dirname(target)) || process.env.CLAUDE_PROJECT_DIR || cwd;
  const rel = relative(resolve(repoRoot), target).split(/[\\/]/).join('/');

  // Outside the repo, or an unresolvable path -> not ours to police.
  if (!rel || rel.startsWith('..') || /^[a-zA-Z]:/.test(rel)) allow();
  const config = readConfig(repoRoot);
  if (!underAny(rel, config.guardedDocRoots)) {
    if (/\.(md|txt)$/i.test(rel)) allow();
    if (underAny(rel, config.exemptRoots)) allow();
    if (config.guardedRoots.length > 0 && !underAny(rel, config.guardedRoots)) allow();
  }

  // --- Everything below only runs for a genuine code write. -----------------

  let log = '';
  try {
    log = readFileSync(join(repoRoot, LOG_PATH), 'utf8');
  } catch {
    allow(); // Constraint 1: no log file -> nothing to satisfy, so never block.
  }

  // Only table rows count. Anchoring to the leading pipe keeps prose dates, the
  // `YYYY-MM-DD HH:MM` format example, and the `date -u` snippet from matching.
  const stamps = [...log.matchAll(/^\|\s*(\d{4}-\d{2}-\d{2})[ T](\d{2}:\d{2}(?::\d{2})?)/gm)]
    .map((m) => Date.parse(`${m[1]}T${m[2].length === 5 ? `${m[2]}:00` : m[2]}Z`))
    .filter(Number.isFinite);

  if (stamps.length === 0) {
    fail(rel, `No intake has ever been recorded in ${LOG_PATH}.`);
  }

  const windowHours = config.intakeWindowHours;
  const newest = Math.max(...stamps);
  const hoursAway = Math.abs(Date.now() - newest) / 3_600_000;
  if (hoursAway <= windowHours) allow();

  fail(rel, `The most recent intake is ${Math.round(hoursAway)}h old (window: ${windowHours}h).`);
}

function readConfig(repoRoot) {
  const config = { ...DEFAULTS };
  try {
    const raw = JSON.parse(readFileSync(join(repoRoot, CONFIG_PATH), 'utf8'));
    if (Array.isArray(raw.guardedRoots)) config.guardedRoots = raw.guardedRoots.filter(isRoot);
    if (Array.isArray(raw.exemptRoots)) config.exemptRoots = raw.exemptRoots.filter(isRoot);
    if (Array.isArray(raw.guardedDocRoots)) config.guardedDocRoots = raw.guardedDocRoots.filter(isRoot);
    if (positive(raw.intakeWindowHours)) config.intakeWindowHours = Number(raw.intakeWindowHours);
  } catch {
    // Missing or malformed config -> defaults.
  }
  const envWindow = process.env.INTAKE_GATE_WINDOW_HOURS;
  if (envWindow !== undefined && positive(envWindow)) config.intakeWindowHours = Number(envWindow);
  return config;
}

function isRoot(value) {
  return typeof value === 'string' && value.trim() !== '';
}

function positive(value) {
  const n = Number(value);
  return value !== null && value !== '' && Number.isFinite(n) && n > 0;
}

// True when `rel` is a root itself or sits below it. Roots may be nested
// (`src/server`); comparison is case-insensitive on Windows.
function underAny(rel, roots) {
  const norm = (p) => {
    const s = p.split(/[\\/]/).filter(Boolean).join('/');
    return process.platform === 'win32' ? s.toLowerCase() : s;
  };
  const path = norm(rel);
  return roots.some((root) => {
    const r = norm(root);
    return r !== '' && (path === r || path.startsWith(`${r}/`));
  });
}

function findRepoRoot(dir) {
  for (let d = dir; ; d = dirname(d)) {
    if (existsSync(join(d, LOG_PATH))) return d;
    if (dirname(d) === d) return null;
  }
}

function fail(rel, reason) {
  process.stderr.write(
    [
      'BLOCKED by the feature-intake gate.',
      '',
      reason,
      '',
      `You are about to write ${rel}, a guarded path. Per docs/FEATURE_INTAKE.md,`,
      'classify the work first and emit the lane block to the user, then add a row',
      `at the top of the ${LOG_PATH} table:`,
      '',
      '  | Date (UTC) | Type | Lane | Summary |',
      '  | 2026-01-31 09:15 | change request | normal | One line naming the work |',
      '',
      '  types: change request | new spec | spec slice | new initiative |',
      '         maintenance request | harness improvement',
      '  lanes: tiny | normal | high-risk',
      '',
      "  timestamp (UTC):  date -u '+%Y-%m-%d %H:%M'",
      '',
      'This fires on the prompt that first asks for code — including when a',
      'conversation that began as a question turns into an implementation.',
      '',
      'Exempt roots and markdown are never gated. To disable: INTAKE_GATE_OFF=1.',
    ].join('\n'),
  );
  process.exit(BLOCK);
}
