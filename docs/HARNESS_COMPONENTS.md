# Harness Components

What covers each harness responsibility in this repository, and where coverage
is thin. Use it to find the right file fast, and to attribute a failure to a
component rather than to "the harness" in general.

Status values:

- **Covered**: an explicit file or mechanism exists and is actually used.
- **Partial**: support exists but is incomplete, manual, or unproven in use.
- **Missing**: no meaningful support.

"Actually used" is the bar. A mechanism that exists and sits idle is Partial at
best; see the note on capability versus use in `docs/HARNESS_MATURITY.md`.

## Responsibility Map

| # | Responsibility | Status | Where it lives | Gap |
| --- | --- | --- | --- | --- |
| 1 | Task specification | Covered (2026-10-25: package-exit intake and high-risk story exercised) | `docs/FEATURE_INTAKE.md`, `docs/INTAKE_LOG.md`, `docs/templates/story.md`, `docs/templates/high-risk-story/`, `docs/stories/` | keep future stories narrow |
| 2 | Context selection | Partial (2026-10-25: `CONTEXT_RULES.md` filled; index plus 3 repository files and 3 Unity pack seeds) | `docs/CONTEXT_RULES.md`, `CLAUDE.md` imports, `.claude/knowledge/00-Master-Index.md` | keep 02/03 true as the plan moves code |
| 3 | Task routing | Covered (2026-10-25: router exercised on package exit; current roles match the thin template) | `.claude/skills/unity-task-router/`, `.claude/agents/` | keep derived-project refresh guidance current |
| 4 | Design | Covered (2026-10-25: thin-template and package-boundary decisions recorded) | `docs/decisions/`, `docs/templates/high-risk-story/design.md`, `docs/ARCHITECTURE.md` | record only new decisions, not restatements |
| 5 | Tool access | Covered (2026-10-25: Unity CLI, Addressables setup/build, and Python audit commands proven) | `docs/TOOL_REGISTRY.md`, `.claude/skills/`, `tools/addressables/`, `.claude/settings.json` | platform build tooling waits for a selected target |
| 6 | Project memory | Covered (2026-10-25: knowledge 01–04, decisions, Addressables plan, and package-exit story updated) | `docs/decisions/`, `docs/stories/`, `.claude/knowledge/`, `Learnings.md`, `docs/LEARNINGS-SUMMARY.md` | harvest repeatable friction when it appears |
| 7 | Task state | Covered (2026-10-25: package-exit story and C1–C6 matrix rows) | `docs/TEST_MATRIX.md`, story status, `docs/stories/` | keep status/evidence synchronized |
| 8 | Verification | Covered (2026-10-25: compile, 10 EditMode, 1 PlayMode boot, Addressables static/build/audit/diff, and fresh-clone import/test recorded) | `docs/TEST_MATRIX.md`, `docs/templates/validation-report.md`, the reviewer role | platform/device/HAR proof waits for a selected target and remote feature |
| 9 | Observability | Partial (2026-10-25: trace gate installed; one trace row (the bootstrap)) | `docs/TRACE_LOG.md`, `docs/TRACE_SPEC.md`, `.claude/hooks/trace-gate.js` | trace rows for normal work |
| 10 | Failure attribution | Partial (2026-10-25: Friction column exists; `HARNESS_BACKLOG.md` empty) | the Friction column of `docs/TRACE_LOG.md`, this table, `docs/HARNESS_BACKLOG.md` | first backlog item from friction |
| 11 | Permissions | Partial (2026-10-25: framework `settings.json` hooks and permissions; project hard gates written) | `.claude/settings.json`, `docs/FEATURE_INTAKE.md` hard gates, the Harness Change Policy in `docs/HARNESS.md` | a hard gate exercised by a real task |
| 12 | Entropy auditing | Missing (2026-10-25: no audit run) | `docs/HARNESS_AUDIT.md`, `docs/IMPROVEMENT_PROTOCOL.md`, `harness-bootstrap` refresh mode | first `HARNESS_AUDIT.md` run after a few tasks |
| 13 | Framework feedback | Partial (2026-10-25: `harvest` skill present, never run) | the `harvest` skill, `.agent-runtime/harvest/` | harvest after the first learnings |

## Coverage Summary

Rebaseline 2026-10-25 after package exit: Covered 7/13, Partial 5/13,
Missing 1/13. The remaining product-facing proof gap is deliberate: platform
build and HAR evidence begin after the owner selects a target and the first
remote/partial feature exists.

## Enforcement Points

Everything else in the harness is advisory. These are the mechanisms that stop
an agent:

| Mechanism | Enforces | Fails |
| --- | --- | --- |
| `.claude/hooks/intake-gate.js` | an intake is logged before the first code write | open, on every infrastructure path |
| `.claude/hooks/trace-gate.js` | a reminder when normal or high-risk work has no trace | open; reminds once, never blocks twice |
| `.claude/settings.json` permissions | which tools run without a prompt | closed: the user is asked |

<!-- stack: add a row for each further mechanism this repository has (CI checks, pre-commit hooks, verify scripts wired into CI, branch protection), with what it enforces and how it fails. Leave this comment if there are none. -->

A harness rule not backed by one of these is a convention. Conventions work
until they do not, which is why the intake gate exists.
