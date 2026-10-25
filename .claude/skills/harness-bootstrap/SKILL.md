---
name: harness-bootstrap
description: 'Finish installing the copied agent harness in this repository, or refresh it later. Detects the stack from evidence, creates or upgrades the task router skill, writes the first knowledge base and path-scoped rules from the real code, fills every `<!-- stack: ... -->` placeholder, sets the intake-gate code roots, and applies the installer''s pending merges. Use right after the harness is copied in ("bootstrap the harness", "finish the harness install", `.claude/harness-manifest.json` lists pendingMerges, or `<!-- stack:` placeholders remain), and again in refresh mode when modules, skills, scripts, or tooling have drifted ("refresh the harness", "harness drift"). Never deletes or renames an existing skill, rule, or agent.'
---

# Harness Bootstrap

The installer copies the harness verbatim. This skill makes it true for this
repository: every statement it writes must come from evidence in the repository
(files, manifests, configs, existing docs), cited by path. Where evidence is
missing, write `TODO(harness-bootstrap): <what is missing>` instead of guessing.

## Lifecycle

Where this skill sits in the harness's life in a repository:

1. **Init**: the installer copies core and the selected packs, and records
   them in `.claude/harness-manifest.json`.
2. **Onboarding**: `onboard-repository` maps the repository and traces one real
   path; this skill makes the harness true for it (router, knowledge, rules,
   placeholders, gate roots, merges).
3. **Tuning**: the repository adds its own skills, rules, roles, and knowledge
   (`.claude/ONBOARDING.md`, section "Extending The Harness").
4. **Instruction**: `CLAUDE.md`, `AGENTS.md`, and the router are reviewed and
   committed by the owner so the whole team shares them.
5. **Loop**: every task runs intake → route → work → prove → trace; learnings
   are distilled (`learning-distillation`) and generic improvements harvested
   back to the framework (`harvest`). Refresh mode of this skill catches drift.

## Hard Rules

- **Never delete or rename** an existing skill, rule, agent, knowledge file, or
  doc. Upgrade in place, append, or report. Inventory before and after (step 1
  and step 10) and prove the "after" set is a superset of the "before" set.
- **Never rewrite owner text.** Existing prose, table rows, and settings stay.
  Add sections or rows, replace only `<!-- stack: ... -->` comments and
  `TODO(harness-bootstrap)` markers, and report anything stale as a proposal.
- **Team instructions win.** If a tracked `CLAUDE.md` or `AGENTS.md` says
  something different from the harness, keep the team's text and report the
  conflict.
- **No product code, no installs, no deploys.** Do not install dependencies,
  start long-running services, run migrations, or touch CI/branch settings.
  Running a cheap, read-only proof command once to verify it exists is allowed;
  if it needs an install or a service, record it as "not run" with the reason.
- **No commits.** Leave the changes in the working tree for the owner.

## Mode

- **Bootstrap mode** — `.claude/knowledge/00-Master-Index.md` has no
  "Harness bootstrap" record, or `<!-- stack:` placeholders remain outside this
  skill's own folder. Run steps 1–10.
- **Refresh mode** — the bootstrap record exists. Run "Refresh Mode" below.

Announce the mode, then follow the steps in order.

## Bootstrap Procedure

### 1. Read the current state

Read, before any write:

- `.claude/harness-manifest.json` — `pendingMerges` is a list of
  `{ "path", "incoming" }`: a file the installer did not overwrite because it
  already existed, with the framework version staged at `incoming` (under
  `.agent-runtime/harness-incoming/`). `merged` maps a path to the hash of the
  framework version already merged. `packs` lists the installed packs: either
  names (`"go-backend"`) or objects with a `name` and the pack's
  `guardedRoots`, `proofTypes`, and `routerDomains`; accept both shapes. No
  `packs` key means core only. Do not edit the `files` map.
- `CLAUDE.md`, `AGENTS.md`, `README*`, `CONTRIBUTING*`, and any
  `docs/ARCHITECTURE.md`.
- `CLAUDE.local.md`, if present. A copy left by the retired link overlay points
  at `.paul-ai/framework/` or `.paul-ai/state/`, which no longer exist. Move any
  project-specific content it holds into `CLAUDE.md` (or the knowledge base),
  then show the user the stale file and propose deleting it; never delete it
  without their yes.
- Every `SKILL.md` under `.claude/skills/` (frontmatter and headings), plus any
  other agent skill directory the repository keeps (for example
  `.agents/skills/`).
- `.claude/rules/*.md`, `.claude/agents/*.md`, `.claude/knowledge/*`,
  `.claude/settings.json`, `.claude/harness.config.json`.

Record the **inventory**: the full path list of skills, rules, agents, and
knowledge files. Step 10 compares against it.

### 2. Detect the stack

Start from the installed packs, then confirm against evidence. For each pack in
`packs`, take its hints: the `guardedRoots`, `proofTypes`, and `routerDomains`
recorded in the manifest when present, else the stack-hints row the pack maps
to (`references/stack-hints.md`, "Installed Packs"). Hints seed the router
domains (step 4), the proof rows (step 7), and `guardedRoots` (step 8); keep
only what the repository's files confirm.

Then use `references/stack-hints.md` to match detection signals against real
files (build manifests, lockfiles, engine project files, top-level
directories). A monorepo may have several stacks; record each with its root.
Pick the package manager from the lockfile, not from habit. A stack the
evidence shows but no installed pack covers is reported, with the pack that
would cover it; a pack installed with no matching evidence is reported too.

For each stack record: name, root, language/framework versions read from
manifests, the evidence paths, and the code roots. If no row matches, use the
generic row and say why. Write this into the "Stack" section of the knowledge
index (step 4).

### 3. Log the intake

Before the first write, classify per `docs/FEATURE_INTAKE.md` (a bootstrap is a
harness change: `normal` lane, `high-risk` if merging hooks into a
`.claude/settings.json` that already wires hooks or permissions), emit the lane
block, and add a row to `docs/INTAKE_LOG.md` following that file's header.
Harness and markdown paths are not gated by `.claude/hooks/intake-gate.js`, so
this is discipline, not a hook.

### 4. Router

Find an existing router: a skill named `*-task-router` or `route-*-tasks`, or
any skill whose description says it routes, classifies, or dispatches tasks.

- **One exists → upgrade it in place** to `references/router-skeleton.md`.
  Keep its name, folder, and every existing route, domain, hard gate, skill
  reference, agent reference, and reference file. Extend its description
  triggers rather than replacing them. Add only the skeleton parts it lacks:
  the routing line, lanes, scoring, role sequence, role ↔ context-phase map,
  spawn gate, and "inline when unsure". If it already has its own scoring
  rubric, keep it and map its thresholds onto the skeleton's lane/spawn/design
  bands instead of replacing it. If it lives in a directory Claude Code does not
  load (for example `.agents/skills/`), upgrade it there and report that Claude
  Code discovers skills only under `.claude/skills/`; never move it.
- **More than one exists** → upgrade the one `CLAUDE.md`/`AGENTS.md` names, and
  report the others.
- **None exists → create `.claude/skills/<project>-task-router/SKILL.md`** from
  the skeleton. `<project>` is the repository's short name in kebab case (from
  the root package/module name, else the directory name).

Domains come from the installed packs' `routerDomains` and the stack row's
typical domains, kept only when the code has that area (cite the root that
proves it), plus the standing rows the skeleton requires. Point each row at a
skill or agent that exists; never invent one. Every existing skill must be
reachable from at least one row.

Fill the router's role table from `.claude/agents/`:

| Role | Agent |
| --- | --- |
| design | `architect` |
| implement | the pack implementers present (`<stack>-*` agents that write code), each with the domains it serves; inline when none fits |
| review | the pack's stack reviewer (the `*-reviewer` agent the pack ships, for example `go-backend-reviewer`) when present, else `code-reviewer` |
| document | `documenter` |

Every agent in `.claude/agents/` must be reachable from a domain row or a role.
A project agent that predates the harness keeps its existing routes and is
mapped to the role it plays.

Fill the Roles table's `Model` column: `session` for every role, unless
`packs` includes `agent-team`. In that case, add the `test` and `escalate` rows
and the tiers from `docs/TEAM.md` (roster and "Router Rows"), confirm every high-risk
domain's route is inline, and fill `docs/TEAM.md` (its roster equals the Roles
table, plus one surface → skill → knowledge row per domain). Never write a
tier into a framework agent's `model:` frontmatter; the router passes it.

Then make the harness name the router: replace "the project's task router
skill" in the `## Harness` section of `CLAUDE.md` and the `<project>-task-router`
mention in `AGENTS.md` with the real skill name, only where that wording appears.

### 5. Knowledge base

Write `.claude/knowledge/00-Master-Index.md` and 3–6 first files using
`references/knowledge-index-template.md`: entry & app state, data models,
wiring/DI or module composition, how-to-add-X recipes, build & deploy (and one
domain deep dive if the code clearly has a central subsystem).

- **Code discovery:** use the codebase-memory MCP tools first when available —
  `index_status` (index the repository if it is not indexed; this writes only
  the tool's own store), `get_architecture`, `search_graph`, `trace_path`,
  `get_code_snippet`. Confirm every claim with Grep/Read before writing it.
  Without the MCP, use Glob/Grep/Read.
- Cite a file path (and symbol or line) for every claim. Mark behavior read from
  code as observed, not as policy.
- Reference existing skills, rules, and docs instead of copying their content.
- Existing knowledge files stay: list them in the index and number new files
  after them.
- Knowledge seeds shipped by packs (generic to the stack) are listed in the
  index as they are; repository-specific facts go in new files, not into a
  pack seed.

Write the bootstrap record into the index (date, mode, stack, evidence,
revision) — refresh mode keys off it.

### 6. Rules

Write `.claude/rules/<topic>.md` per `references/rule-template.md`, with
`paths:` frontmatter globs for the stack's languages. Only from conventions with
evidence: lint/format/type configs, analyzer settings, `.editorconfig`, existing
skills or docs that state a convention. Mark each rule **gate** (stated by the
owner as mandatory, or enforced by a configured tool) or **guideline**. Do not
promote a code pattern you merely observed into a gate; a convention seen in
code with no stated authority is at most a guideline that cites the examples.
Keep existing rules untouched; if one lacks `paths:` it loads every session —
report it, do not add frontmatter.

### 7. Fill the placeholders

```bash
grep -rn --exclude-dir=harness-bootstrap "<!-- stack:" CLAUDE.md AGENTS.md docs .claude
```

Each comment states what belongs there. Replace the whole comment with the
answer from evidence. Required coverage:

- `docs/TEST_MATRIX.md` — the proof table: one row per proof kind (compile or
  type-check, unit test, integration/e2e, lint, build, manual/device check) with
  the real command and the code area it covers.
- `docs/TOOL_REGISTRY.md` — one row per command that exists (a package script,
  Make target, CLI, verify script, MCP tool configured for this repo). Verify the
  script/target is defined before adding the row.
- `docs/FEATURE_INTAKE.md` — the project risk checklist and hard gates, naming
  concrete paths, modules, symbols, and config keys.
- `docs/CONTEXT_RULES.md` — triggers (path or module → files to read first) and
  the build/test commands.
- `docs/HARNESS.md`, `CLAUDE.md`, `AGENTS.md`, `docs/HARNESS_AUDIT.md`,
  `docs/IMPROVEMENT_PROTOCOL.md`, `docs/templates/**` — stack facts, proof
  ladder, integration branch, surfaces, design layers.
- `.claude/skills/*/SKILL.md` — the "In this repository" placeholders (for
  example `encode-invariant`, `onboard-repository`).
- `docs/ONBOARDING.md` — prerequisites with versions, clone, setup, run, test,
  branch and PR route, where things live, troubleshooting.
- `docs/ARCHITECTURE.md` — system overview, module map, dependency direction,
  entry points, data and state, contracts, build variants, frozen areas.
- `docs/HARNESS_COMPONENTS.md` and `docs/HARNESS_MATURITY.md` — baseline them:
  set each status from what the repository shows today (files that exist but
  have no recorded use are Partial, not Covered; claim no maturity level the
  files do not prove), with dated evidence and the next-rung costs.
- `docs/AGENT_COMPATIBILITY.md` — project-specific Claude concepts and tool
  setup notes for non-Claude agents.
- `.claude/ONBOARDING.md` — the router's name and domains, and every agent in
  `.claude/agents/` beyond the core roles.
- `docs/TEAM.md` (only with the `agent-team` pack) — roster agents and tiers
  equal to the router's Roles table; one surface → skill → knowledge row per
  router domain.

Leave no placeholder unless evidence is missing; then replace it with
`TODO(harness-bootstrap): <what is missing and where it would come from>`. A
placeholder whose comment says "leave this comment if none" may stay.

### 8. Intake-gate roots

Edit `.claude/harness.config.json`: set `guardedRoots` to the repository's code
roots from step 2 (source, tests, scripts, build/CI config directories),
starting from the installed packs' `guardedRoots` and keeping only roots that
exist. Keep
`exemptRoots` as installed unless a guarded root contains generated or vendored
output — then add that path and say why. Set `guardedDocRoots` from the installed
packs' `GuardedDocRoots` (prose that is the product, such as `docs/design` in a
game-design repository; markdown there is gated too), keeping only roots that
exist. Keep every other key. Re-parse the file
to prove it is valid JSON (`node -e "JSON.parse(require('fs').readFileSync('.claude/harness.config.json','utf8'))"`).

### 9. Pending merges

Read `references/merge/README.md` first; it is authoritative for each snippet's
merge rule. For each `pendingMerges` entry:

| Path | Merge source | Rule |
| --- | --- | --- |
| `CLAUDE.md` | `references/merge/claude-harness-section.md` | add or update the `## Harness` section only |
| `.claude/settings.json` | `references/merge/settings-hooks.json` and `references/merge/settings-permissions.json` | merge as JSON under `hooks` (no duplicate `command`) and `permissions.allow` (no duplicate entry; never override `deny` or `ask`) |
| `.gitattributes` | `references/merge/gitattributes-lines.txt` | append missing lines; never override a team merge driver |
| `.gitignore` | the staged `incoming` file | append its missing lines (`/.agent-runtime/`); never remove or reorder team lines |
| `AGENTS.md` | the staged `incoming` file | add incoming sections whose headings are absent; never alter existing ones |
| anything else | — | do not guess; leave it pending and report it |

Never remove existing content. Show the user each diff before writing. When a
merge is done (or already present — the merge would change nothing), remove
that entry from `pendingMerges`, add `"<path>": "<sha256 of the incoming file>"`
to the `merged` map (lowercase hex SHA-256 of the staged framework file, e.g.
`(Get-FileHash -Algorithm SHA256 <incoming>).Hash.ToLower()`), and keep every
other manifest key byte-for-byte; re-parse the manifest. The installer then
stays quiet about that file until the framework ships a new version of it. The
staged file under `.agent-runtime/` may stay.

### 10. Verify, trace, report

- Inventory again: every path from step 1 still exists.
- The step 7 grep returns only comments that allow staying; list every
  `TODO(harness-bootstrap)` (`grep -rn --exclude-dir=harness-bootstrap "TODO(harness-bootstrap)" CLAUDE.md AGENTS.md docs .claude`).
- Every JSON file touched re-parses; `node --check .claude/hooks/*.js` passes.
- `git status --short` shows only harness files (no product code).
- Add a `docs/TRACE_LOG.md` row per `docs/TRACE_SPEC.md`, citing the intake
  timestamp, with concrete friction.
- Report: installed packs and stack detected (with evidence, and any mismatch
  between them), router created or upgraded (routes kept/added, role table),
  knowledge and rule files written, placeholders filled, merges
  applied, `guardedRoots`, and every TODO left with what would resolve it. Note
  that filled framework files now differ from the installed copy, so the
  installer will keep them as local edits on reinstall.

## Refresh Mode

Run when the bootstrap record exists. Same hard rules; refresh only appends.

1. Read the state (step 1) and re-detect the stack (step 2).
2. Compute drift against the knowledge index and router:
   - top-level modules, packages, apps, or services not in "Analyzed
     directories", and listed ones that no longer exist;
   - skills, agents, and rules not reachable from a router row or role;
   - packs added to or removed from `packs` in the manifest since the last
     run, and their hints not yet reflected in domains, proof rows, or
     `guardedRoots`;
   - package scripts, Make targets, or CLIs not in `docs/TOOL_REGISTRY.md`;
   - new `pendingMerges` entries;
   - `TODO(harness-bootstrap)` markers whose evidence now exists.
3. Log an intake row (`tiny` when only rows are appended, else `normal`).
4. Apply: append index rows and new knowledge files, append router rows for
   new skills/agents, add new code roots to `guardedRoots`, add tool and proof
   rows, resolve TODOs, apply pending merges (step 9).
5. Do not edit owner-written text. A removed module, a stale path, or a row
   that now looks wrong is reported as a proposal with its evidence, not
   changed.
6. Update the bootstrap record's "last refresh" line, verify (step 10), add a
   trace row, and report the drift found and what was appended.

## References

- `references/stack-hints.md` — detection signals, domains, proof commands,
  `guardedRoots`, rule globs, and tools per stack, and the pack-to-row map.
- `references/router-skeleton.md` — the router shape, scoring, roles, spawn
  gate, and upgrade checklist.
- `references/knowledge-index-template.md` — the index shape and what each
  first knowledge file contains.
- `references/rule-template.md` — `paths:` frontmatter, gate vs guideline,
  globs per language.
- `references/merge/` — merge snippets and their rules.
