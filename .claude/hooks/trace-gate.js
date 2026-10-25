#!/usr/bin/env node
/**
 * Stop gate: remind, once, when normal or high-risk work has no trace.
 *
 * Why this exists: intake is gated at the START of a task (intake-gate.js) and is
 * reliably recorded; the closing artifacts were prose-only and leaked. In long
 * conversational sessions made of many small asks there is no single "task done"
 * moment, and context compaction keeps outcomes but drops the friction that
 * TRACE_LOG exists to capture. Prose in an always-loaded file was not enough.
 *
 * Rule: a normal or high-risk row in docs/INTAKE_LOG.md is "traced" when its
 * timestamp string (`YYYY-MM-DD HH:MM`) appears anywhere in docs/TRACE_LOG.md —
 * by convention in the Intake column. One trace row may cite several intakes.
 *
 * Design constraints, in priority order:
 *  1. FAIL OPEN, ALWAYS. Any read/parse failure allows the stop.
 *  2. REMIND ONCE per intake. Each intake is named in at most one reminder
 *     (remembered in a temp file keyed by a hash of the repo path), so a session
 *     that is still mid-task is not nagged on every turn. `stop_hook_active` is
 *     honoured so the reminder can never loop.
 *  3. Give work time to finish: only intakes older than `traceMinAgeMinutes`
 *     (default 90) and newer than `traceWindowHours` (default 72) count.
 *  4. Only the current author's rows: intakes added by another git user (merged in
 *     from another branch) never produce a reminder. When git is unavailable or
 *     fails, no author filter is applied.
 *
 * Configuration: `<repoRoot>/.claude/harness.config.json`
 *   { "traceWindowHours": 72, "traceMinAgeMinutes": 90 }
 * Env overrides: TRACE_GATE_WINDOW_HOURS, TRACE_GATE_MIN_AGE_MIN,
 * TRACE_GATE_STATE_DIR (where the remind-once file lives; default the OS temp dir),
 * TRACE_GATE_AUTHOR_FILTER=0 (skip the git author filter). TRACE_GATE_OFF=1 disables.
 */
const { existsSync, readFileSync, writeFileSync } = require('node:fs');
const { createHash } = require('node:crypto');
const { dirname, join, resolve } = require('node:path');
const { execFileSync } = require('node:child_process');
const { tmpdir } = require('node:os');

const ALLOW = 0;
const BLOCK = 2;
const INTAKE = 'docs/INTAKE_LOG.md';
const TRACE = 'docs/TRACE_LOG.md';
const CONFIG_PATH = '.claude/harness.config.json';
const DEFAULTS = { traceWindowHours: 72, traceMinAgeMinutes: 90 };

const allow = () => process.exit(ALLOW);

try {
  if (process.env.TRACE_GATE_OFF === '1') allow();

  const event = JSON.parse(readFileSync(0, 'utf8') || '{}');
  if (event.stop_hook_active) allow();

  const start = event.cwd || process.env.CLAUDE_PROJECT_DIR || process.cwd();
  const repoRoot =
    findRepoRoot(resolve(start)) ||
    (process.env.CLAUDE_PROJECT_DIR && findRepoRoot(resolve(process.env.CLAUDE_PROJECT_DIR)));
  if (!repoRoot) allow();

  const { windowHours, minAgeMin } = readConfig(repoRoot);
  const intake = readFileSync(join(repoRoot, INTAKE), 'utf8');
  const trace = readFileSync(join(repoRoot, TRACE), 'utf8');
  const now = Date.now();

  const mine = process.env.TRACE_GATE_AUTHOR_FILTER === '0' ? null : rowsByMe(repoRoot);
  const owed = [];
  const rowPattern = /^\|\s*(\d{4}-\d{2}-\d{2} \d{2}:\d{2})\s*\|\s*([^|]+?)\s*\|\s*(normal|high-risk)\s*\|\s*([^|]*)/gim;
  for (const m of intake.matchAll(rowPattern)) {
    const [, stamp, , lane, work] = m;
    const ageMin = (now - Date.parse(`${stamp.replace(' ', 'T')}:00Z`)) / 60_000;
    if (!Number.isFinite(ageMin) || ageMin < minAgeMin || ageMin > windowHours * 60) continue;
    if (trace.includes(stamp)) continue;
    if (mine && !mine.has(stamp)) continue;
    owed.push({ stamp, lane: lane.toLowerCase(), work: work.replace(/\s+/g, ' ').trim().slice(0, 110) });
  }
  if (owed.length === 0) allow();

  const stateDir = process.env.TRACE_GATE_STATE_DIR || tmpdir();
  const repoHash = createHash('sha1').update(resolve(repoRoot)).digest('hex').slice(0, 12);
  const statePath = join(stateDir, `claude-trace-gate-${repoHash}.json`);
  let reminded = [];
  try {
    reminded = JSON.parse(readFileSync(statePath, 'utf8'));
    if (!Array.isArray(reminded)) reminded = [];
  } catch {
    reminded = [];
  }
  const fresh = owed.filter((o) => !reminded.includes(o.stamp));
  if (fresh.length === 0) allow();
  try {
    writeFileSync(statePath, JSON.stringify([...reminded, ...fresh.map((o) => o.stamp)].slice(-500)));
  } catch {
    allow(); // cannot remember the reminder -> do not risk repeating it forever
  }

  process.stderr.write(
    [
      'Trace reminder (shown once per intake): normal/high-risk work below has no',
      `row in ${TRACE} citing it.`,
      '',
      ...fresh.map((o) => `  intake \`${o.stamp}\` [${o.lane}] ${o.work}`),
      '',
      'If that work is finished (or paused), add a row at the top of the TRACE_LOG',
      'table before ending — one row may cover several intakes of the same task.',
      'Format and depth rules: docs/TRACE_SPEC.md.',
      '',
      '  | Date (UTC) | Intake | Task | Lane | Outcome | Changed | Friction |',
      '  | 2026-01-31 14:05 | `2026-01-31 09:15` | Task | normal | Outcome | Changed | Friction |',
      '',
      'The Friction column is the point: what the harness made harder than it',
      'should have been, and where each lesson was written down (rule, skill,',
      'memory, check) or that it was not. Friction this session hit that is still',
      'unfixed also goes to docs/HARNESS_BACKLOG.md (see docs/IMPROVEMENT_PROTOCOL.md).',
      '',
      'Trace only work THIS session did. Rows from another session are not yours to',
      'describe — leave them. If the work is still in progress, say so and carry',
      'on. This reminder will not repeat.',
      'To disable: TRACE_GATE_OFF=1.',
    ].join('\n'),
  );
  process.exit(BLOCK);
} catch {
  allow();
}

function readConfig(repoRoot) {
  let windowHours = DEFAULTS.traceWindowHours;
  let minAgeMin = DEFAULTS.traceMinAgeMinutes;
  try {
    const raw = JSON.parse(readFileSync(join(repoRoot, CONFIG_PATH), 'utf8'));
    if (nonNegative(raw.traceWindowHours)) windowHours = Number(raw.traceWindowHours);
    if (nonNegative(raw.traceMinAgeMinutes)) minAgeMin = Number(raw.traceMinAgeMinutes);
  } catch {
    // Missing or malformed config -> defaults.
  }
  if (nonNegative(process.env.TRACE_GATE_WINDOW_HOURS)) windowHours = Number(process.env.TRACE_GATE_WINDOW_HOURS);
  if (nonNegative(process.env.TRACE_GATE_MIN_AGE_MIN)) minAgeMin = Number(process.env.TRACE_GATE_MIN_AGE_MIN);
  return { windowHours, minAgeMin };
}

function nonNegative(value) {
  if (value === undefined || value === null || value === '') return false;
  const n = Number(value);
  return Number.isFinite(n) && n >= 0;
}

// Intake stamps added by the current git user (or not yet committed), so a
// teammate's rows merged in from another branch never produce a reminder.
// null = unknown (git missing or failed) -> no filter, still fail-open overall.
function rowsByMe(repoRoot) {
  try {
    const opts = { cwd: repoRoot, encoding: 'utf8', stdio: ['ignore', 'pipe', 'ignore'], timeout: 5000 };
    const me = execFileSync('git', ['config', 'user.email'], opts).trim();
    if (!me) return null;
    const blame = execFileSync('git', ['blame', '--line-porcelain', INTAKE], { ...opts, maxBuffer: 64 << 20 });
    const stamps = new Set();
    let mail = '';
    for (const line of blame.split('\n')) {
      if (line.startsWith('author-mail ')) mail = line.slice(13, -1);
      else if (line.startsWith('\t|')) {
        const m = line.match(/^\t\|\s*(\d{4}-\d{2}-\d{2} \d{2}:\d{2})/);
        if (m && (mail === me || mail === 'not.committed.yet')) stamps.add(m[1]);
      }
    }
    return stamps;
  } catch {
    return null;
  }
}

function findRepoRoot(dir) {
  for (let d = dir; ; d = dirname(d)) {
    if (existsSync(join(d, INTAKE))) return d;
    if (dirname(d) === d) return null;
  }
}
