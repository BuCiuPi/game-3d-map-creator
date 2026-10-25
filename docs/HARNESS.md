# Harness

This repository runs an operating harness for coding agents. The product is what
users touch. The harness is what agents touch — the docs, conventions, rules, and
routing that let an agent understand intent, work inside a risk lane, and prove
the change before claiming it is done.

## Mental Model

```text
+------------------+
| Human intent     |
+------------------+
         |
         v
+------------------+
| Feature intake   |   docs/FEATURE_INTAKE.md — classify + choose lane
+------------------+
         |
         v
+------------------+
| Task routing     |   the project's task router skill — domain + role
+------------------+
         |
         v
+------------------+
| Agent work loop  |   inline skill or spawned specialist agent
+------------------+
         |
         v
+------------------+
| Product delta    |   code, tests, assets, data, configuration
+------------------+
         |
         v
+------------------+
| Validation proof |   the proof types in docs/TEST_MATRIX.md
+------------------+
         |
         v
+------------------+
| Harness delta    |   docs, conventions, rules, decisions that ease the next task
+------------------+
```

Every task has two possible outputs:

1. **Product delta** — code, tests, assets, data, configuration, or the
   `.claude/knowledge/` docs that describe them.
2. **Harness delta** — process docs, templates, conventions, rules, or decision
   records (`docs/decisions/`) that make the next task easier. Not every task
   produces one, but every task should ask whether it should.

## Harness Principles

These four rules hold regardless of stack:

1. **Markdown in the repository is the only source of truth.** There is no
   database, no CLI state, and nothing to bootstrap. A fresh clone has the
   complete harness state because every artifact is a tracked file.
2. **Dispatch is role-based.** Work moves through four roles: **design** →
   **implement** → **review** → **document**. A model name attached to a step is
   a role label, not an effort hint; the role picks the model.
3. **Docs live on the branch of the work they describe.** Stories, decisions,
   and plans are written on the same branch as the change. There is no standing
   planning branch; docs reach the integration branch the way code does, through
   a reviewed merge.
4. **Agent rules live in `.claude/rules/`** with `paths:` frontmatter naming the
   files they govern — not in ad-hoc folders per agent platform. Non-Claude
   agents read the matching rule before writing code.

## Reference Surfaces

The harness leans on curated knowledge that lives in the repository. Read from
it; keep it current.

- `.claude/knowledge/00-Master-Index.md` — the codebase knowledge base. Load the
  one file relevant to the current task.
- `.claude/rules/` — coding rules, each scoped by `paths:` frontmatter.

- `.claude/knowledge/01-project-brief.md`, `.claude/knowledge/02-runtime-and-wiring.md`, `.claude/knowledge/03-build-and-profiles.md` — purpose, wiring, build and profiles.
- `docs/architecture/template-core-data.md` — the DT/DA contract.
- `artifacts/unity-template-addressables-plan.html` — the owner's plan.

## Task Loop

For every task:

1. **Classify** with `docs/FEATURE_INTAKE.md` — input type, then lane
   (tiny / normal / high-risk). Emit the lane block to the user and append a row
   to `docs/INTAKE_LOG.md` before the first code write.
2. **Route** with the project's task router skill under `.claude/skills/` —
   pick the domain, the cheapest correct path (inline skill vs spawned agent),
   and the role.
3. **Read** only what the lane and phase call for, per `docs/CONTEXT_RULES.md`.
4. **Work inside the chosen lane.** Do not silently expand scope. High-risk work
   needs the high-risk story set, and human confirmation before code when
   direction is ambiguous (`docs/FEATURE_INTAKE.md`).
5. **Prove** the change against `docs/TEST_MATRIX.md`. Never claim a validation
   passed that you did not run.
6. **Before finishing,** ask whether product truth, conventions, architecture
   rules, or recurring failure patterns changed — if so, record the harness
   delta (update a knowledge doc or rule, or add a `docs/decisions/` record). Log
   it in `docs/TRACE_LOG.md` per `docs/TRACE_SPEC.md` (normal and high-risk work
   always; tiny work only when it hit friction or changed harness rules), naming
   any friction concretely — that column is what drives harness improvement.
7. **Report** what changed and what was not attempted.

## Proof In This Repository

Match the proof to the change; `docs/TEST_MATRIX.md` holds the full mapping.

Proof ladder: `compile` on every C# change (Editor recompile or batch mode); `editmode` for `Assets/game.core/` and plain C# logic; `boot`, run the PlayMode boot test or play `00_LoadingScene` until the neutral global UI shell is ready; `editor-evidence` for UI and prefab work; `addressables-build` for group, profile or asset-move changes; `platform`, a device build, deferred until the owner selects a target. A normal clone is complete and needs no submodules. Commands: `docs/TEST_MATRIX.md`.

Record notable validation in the `docs/templates/validation-report.md` shape, in
the PR body, or — for architectural changes — in a `docs/decisions/` record.

## Risk Lanes

Lanes come from `docs/FEATURE_INTAKE.md` and are reused everywhere (intake,
stories, the router's scoring):

- **Tiny** — narrow, low-risk edits. Patch directly; run quick checks. No story.
- **Normal** — story-sized behavior with bounded blast radius. Create/update a
  story from `docs/templates/story.md`; record expected proof.
- **High-risk** — can affect contracts, data, shared framework code, build or
  deployment configuration, or multiple surfaces. Use
  `docs/templates/high-risk-story/`, get human confirmation when direction is
  ambiguous, and record a durable decision in `docs/decisions/` when
  architecture or contracts change.

## Where Docs Live

Docs are written on the branch of the work they describe. A feature branch
carries its story, decision, and proof rows alongside its code; docs-only work
goes on whatever branch the work is happening on. There is no standing planning
branch.

Docs reach the integration branch the way code does, through a reviewed merge.

Integration branch `main` on `hoangphuc25071995/unity-template`. No pull requests so far; commits land on `main` directly. TODO(harness-bootstrap): confirm whether changes should go through pull requests and who reviews.

## Decision Records

When high-risk work changes behavior or architecture — public or cross-service
contracts, module boundaries, auth/session flow, data schemas, dependency
injection or bootstrap roots, or the harness's own rules — record it as
`docs/decisions/NNNN-kebab-title.md` from `docs/templates/decision.md`. A note
buried in a PR is not a durable decision.

## Harness Change Policy

Agents may update directly:

- `.claude/knowledge/` docs to match reality.
- Story files and validation notes tied to the current task.
- Small clarifications to conventions.

Agents should ask for human confirmation before:

- Changing architecture direction.
- Removing or weakening a validation expectation.
- Changing the risk-classification rules in `docs/FEATURE_INTAKE.md`.
- Changing the task-routing policy in the project's task router skill.
- Adding, removing, or materially changing a rule in `.claude/rules/`.

## Growth Rule

The harness grows from friction. When an agent is repeatedly confused, hits a
stale doc, or has to re-derive the same reasoning, it should fix the harness in
the same task when cheap, or note the gap in the final response (and, when it is
a convention, update the relevant knowledge doc or rule).

## Done Definition

A task is done only when:

- The requested change is complete, or the blocker is documented.
- The project builds (or the reason it cannot is stated).
- Proof appropriate to the lane was produced or explicitly deferred with a
  reason.
- Affected knowledge docs, rules, and any story remain current.
- The final response says what changed and what was not attempted.

## Where The Record Lives

Every artifact is tracked in git; a fresh clone has the complete harness state.

| Artifact | File |
| --- | --- |
| Intake classification | `docs/INTAKE_LOG.md` |
| Story packet | `docs/stories/<area>/` or `docs/stories/<name>.md` |
| Proof of behavior | `docs/TEST_MATRIX.md` |
| Durable decision | `docs/decisions/NNNN-*.md` |
| Task trace + friction | `docs/TRACE_LOG.md` |
| Harness improvement | `docs/HARNESS_BACKLOG.md` |
| Equipped tools | `docs/TOOL_REGISTRY.md` |
| Durable learnings | `Learnings.md`, `docs/LEARNINGS-SUMMARY.md` |

`docs/contracts/harness-artifacts-v1.md` states the shape of each and what a
consumer may rely on. `docs/HARNESS_AUDIT.md` is the periodic drift check.
