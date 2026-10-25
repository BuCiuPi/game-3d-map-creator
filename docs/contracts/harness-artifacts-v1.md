# Harness Artifact Contract v1

Where each harness artifact lives, what shape it must hold, and what a consumer
may rely on.

The contract is simple: **the files are the interface.** A consumer is any
agent, script, or human reading them. There is no discovery step, no version
negotiation, and no mutation protocol — reading is `cat`, and writing is an
ordinary edit reviewed like any other.

## Artifact Map

| Artifact | Location | Shape | Appended by |
| --- | --- | --- | --- |
| Intake classification | `docs/INTAKE_LOG.md` | Table row: `\| Date (UTC) \| Type \| Lane \| Summary \|` | Every task, before the first code write |
| Story packet | `docs/stories/<area>/` or `docs/stories/<name>.md` | Markdown from `docs/templates/story.md` or `docs/templates/high-risk-story/` | Normal and high-risk work |
| Proof of behavior | `docs/TEST_MATRIX.md` | Table row: id, area, behavior, proof, status, evidence | Every normal/high-risk story |
| Durable decision | `docs/decisions/NNNN-kebab-title.md` | Markdown from `docs/templates/decision.md` | Architecture, contract, auth, or policy changes |
| Task trace | `docs/TRACE_LOG.md` | Table row `\| Date (UTC) \| Intake \| Task \| Lane \| Outcome \| Changed \| Friction \|`, plus a section for high-risk — see `docs/TRACE_SPEC.md` | Normal and high-risk work |
| Harness improvement | `docs/HARNESS_BACKLOG.md` | Item from the template at the top of that file | Recurring friction |
| Equipped tools | `docs/TOOL_REGISTRY.md` | Table row: tool, kind, capability, presence check, purpose | When the project equips a tool |
| Durable learnings | `Learnings.md`, `docs/LEARNINGS-SUMMARY.md` | Index + dated deltas; distilled summary | When a lesson outlives its task |

## Harness Configuration And Reference Surfaces

| Surface | Location | What it holds |
| --- | --- | --- |
| Gate configuration | `.claude/harness.config.json` | `guardedRoots` and `exemptRoots` for the intake gate (when `guardedRoots` is empty, every path outside `exemptRoots` is guarded; markdown and text files are never gated), plus the gates' time windows (`intakeWindowHours`, `traceWindowHours`, `traceMinAgeMinutes`). A missing key falls back to the hook's default |
| Hooks | `.claude/hooks/intake-gate.js`, `.claude/hooks/trace-gate.js` | The two log parsers — see Parsing Rules |
| Knowledge base | `.claude/knowledge/` | `00-Master-Index.md` plus one file per codebase area |
| Rules | `.claude/rules/` | Coding rules, each scoped by `paths:` frontmatter |
| Task router | `.claude/skills/<project>-task-router/` | The project's task router skill — classifies a task and picks its path and role |

## Guarantees A Consumer May Rely On

1. **Every artifact is tracked in git.** A fresh clone has the complete harness
   state. Nothing needs bootstrapping, installing, or migrating.

2. **Markdown is the only source of truth.** No mirror, no cache, no derived
   store. If two artifacts disagree, that is a bug in the docs, not a sync
   failure — and it is visible in `git diff` rather than hidden in a binary.

3. **Logs are append-only.** `docs/INTAKE_LOG.md` and `docs/TRACE_LOG.md` are
   newest-first and rows are not rewritten after the fact; a correction is a new
   row. Status lives in `docs/TEST_MATRIX.md` and story packets, which *are*
   edited in place.

4. **Decisions are superseded, never deleted.** A decision that stops being true
   gets `Status: Superseded` and a pointer to its replacement. Numbering is
   monotonic; numbers are never reused.

5. **Timestamps are UTC** in the format `YYYY-MM-DD HH:MM` (intake stamps) or
   `YYYY-MM-DD` (trace dates). A trace's `Intake` column cites intake stamps
   verbatim. Consumers that parse them should tolerate absolute time skew rather
   than assuming the writer's clock matched theirs.

## Parsing Rules

Two artifacts are machine-parsed, by two hooks:

- `.claude/hooks/intake-gate.js` (`PreToolUse`) reads `docs/INTAKE_LOG.md` and
  blocks the first write to a guarded code path when no recent intake exists.
  Stamps are matched only on lines beginning with `|`, anchored to the leading
  pipe. Prose dates, format examples, and shell snippets elsewhere in the file
  are deliberately not matched. `INTAKE_GATE_OFF=1` disables it.
- `.claude/hooks/trace-gate.js` (`Stop`) reads both `docs/INTAKE_LOG.md` and
  `docs/TRACE_LOG.md`. It takes the lane of each intake row from the table, and
  treats a normal or high-risk intake as traced when its stamp string appears
  anywhere in `docs/TRACE_LOG.md`. It considers only intakes the current git
  user added, older than `traceMinAgeMinutes` and within `traceWindowHours`, and
  reminds once per untraced intake. `TRACE_GATE_OFF=1` disables it.

Any consumer added later must follow the same discipline: anchor to table
structure, never to loose text, and **fail open**. A parser that blocks work
because a markdown file was formatted slightly differently is worse than the
problem it solves.

## What Is Deliberately Not Here

- **No schema validation.** Nothing rejects a malformed row. Review does.
- **No referential integrity.** An intake stamp in the trace log that matches no
  intake row, or a story link that resolves to nothing, is caught by
  `docs/HARNESS_AUDIT.md`, not by a constraint.
- **No queries.** `grep` and `ls` are the query layer. This is a real
  capability loss, accepted knowingly in exchange for guarantee 1.
