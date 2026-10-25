# Context Engineering Rules

Context rules help agents decide what to read, when to read it, and when to stop.
They are additive to the always-loaded set that `CLAUDE.md` imports.

The goal is not to maximize context. The goal is to put the right information in
the model for the current task phase and risk lane.

## Always Loaded (all lanes)

`CLAUDE.md` and the files it imports are in every session — do not re-read them
on purpose:

- `CLAUDE.md` — project overview, architecture, key patterns.
- `AGENTS.md` — the stable pointer list.
- `docs/FEATURE_INTAKE.md` — lanes and the project risk checklist.

Rules in `.claude/rules/` load automatically when a file matching their `paths:`
frontmatter is touched. A non-Claude agent reads the matching rule by hand
before writing code there.

## Context Phases

### Intake Phase — classify the request and choose a lane

| Source | Tiny | Normal | High-Risk |
| --- | --- | --- | --- |
| `docs/FEATURE_INTAKE.md` | Must | Must | Must |
| `.claude/knowledge/00-Master-Index.md` | Should | Must | Must |
| `docs/HARNESS.md` | Skip | Should | Must |
| Relevant `.claude/knowledge/*.md` (per the index) | Skip if unrelated | Must if behavior changes | Must |
| `docs/decisions/*` | Skip | Should if architecture/contracts touched | Must |
| `docs/architecture/template-core-data.md` and `artifacts/unity-template-addressables-plan.html` for anything touching data assets, Addressables placement or the template boundary | Should | Must | Must |

### Planning Phase — decide the smallest safe approach and expected proof

| Source | Tiny | Normal | High-Risk |
| --- | --- | --- | --- |
| Current files to edit | Must | Must | Must |
| `docs/templates/story.md` | Skip | Must when creating/updating a story | Should |
| `docs/templates/high-risk-story/*` | Skip | Skip unless risk escalates | Must |
| `.claude/knowledge/02-runtime-and-wiring.md` and `.claude/knowledge/03-build-and-profiles.md` | Skip | Should for package/wiring changes | Must |
| `docs/TEST_MATRIX.md` — read the proof table at the top, then grep the area's rows; never read it whole | Should | Must | Must |
| Relevant `docs/decisions/*` | Skip | Should | Must |

### Implementation Phase — read while making the change

| Source | Tiny | Normal | High-Risk |
| --- | --- | --- | --- |
| Files being changed | Must | Must | Must |
| Adjacent files with the same pattern | Should | Must | Must |
| Matching `.claude/rules/*` (by `paths:`) | Must | Must | Must |
| Relevant knowledge doc | Skip if copy-only | Must if behavior changes | Must |
| Contract / auth / build-config source (when touched) | Skip | Should if touched | Must |
| Unrelated docs and history | Skip | Skip | Should only if they affect a decision |

### Validation Phase — prove the change

| Source | Tiny | Normal | High-Risk |
| --- | --- | --- | --- |
| Story acceptance criteria | Should | Must | Must |
| `docs/TEST_MATRIX.md` — read the proof table at the top, then grep the area's rows; never read it whole | Should | Must | Must |
| `docs/templates/validation-report.md` | Skip | Should for notable proof | Must |
| Build check + the tests that cover the area | Must | Must | Must |
| Editor recompile with a clean console + EditMode tests (`game.core.tests.editmode`) + the boot check for bootstrap changes | Must | Must | Must |

### Documentation Phase — record the harness delta

| Source | Tiny | Normal | High-Risk |
| --- | --- | --- | --- |
| The diff this task produced | Skip | Must | Must |
| `docs/FEATURE_INTAKE.md` — which artifact the lane owes | Skip | Must | Must |
| `docs/templates/story.md` | Skip | Must | Should |
| `docs/templates/high-risk-story/*` | Skip | Skip unless risk escalated | Must |
| `docs/templates/decision.md` | Skip | Must when behavior/architecture/contract changed | Must |
| `docs/templates/validation-report.md` | Skip | Should for notable proof | Must |
| Existing `docs/decisions/*` this may supersede or contradict | Skip | Must when writing a decision | Must |
| Knowledge docs and rules the change may have invalidated | Skip | Must | Must |
| Unrelated history, source, or knowledge docs | Skip | Skip | Skip |

Tiny is `Skip` throughout — the `document` role does not fire on the tiny lane
(`docs/FEATURE_INTAKE.md`: "patch directly; no story needed"). The column is
kept for symmetry with the other phases.

This phase writes; it does not decide. If the record requires a rationale nobody
stated, mark the gap rather than inventing one — an invented rationale in a
decision record becomes durable and gets built on.

## Retrieval Triggers

| Trigger | Action |
| --- | --- |
| Task touches shared framework code | Treat as high-risk; read the relevant knowledge doc + prior decisions before editing. |
| Task changes a public or cross-system contract | Read the contract's producer and consumer source before editing; treat the shape as a contract. |
| Task creates, renames, or re-references a module/package | Map the reference graph first; treat as high-risk. |
| Task changes architecture, risk rules, or a contract | Read `docs/HARNESS.md`, `docs/FEATURE_INTAKE.md`, and `docs/decisions/*`; pause if direction is ambiguous. |
| Task writes a decision record | Read `docs/decisions/` for records this one supersedes or contradicts. Two live records ruling the same question is worse than none — mark the older one `Superseded` in the same change, and state narrowly which part is replaced. |
| Task hits stale docs or repeated confusion | Fix the doc if cheap, or note the gap in the final response. |
| `TemplateBootstrap`, boot order, or any new `LifetimeScope` in a derived game | Read `.claude/knowledge/02-runtime-and-wiring.md` first. |
| `Assets/game.core/` | Read `docs/architecture/template-core-data.md` first; the public API is copied by every project. |
| `Assets/AddressableAssetsData/`, asset moves or renames | Read `.claude/rules/asset-structure.md` and the plan; keep GUIDs. |
| `Packages/manifest.json`, `Packages/packages-lock.json`, or a Git UPM dependency | Read `.claude/knowledge/01-project-brief.md` and `docs/architecture/networking-on-demand.md`; treat as high-risk. |
| `ProjectSettings/`, `#if` define symbols | Read `.claude/knowledge/03-build-and-profiles.md` first. |
| Editor menus under `Assets/*/Editor/` | Load `unity-editor-scripting` first. |

## Token Budget Guidance

| Lane | Target harness-context budget | Read shape |
| --- | --- | --- |
| Tiny | ~2K tokens | Intake doc, the exact file being changed. |
| Normal | ~5K tokens | Intake, the one relevant knowledge doc, matching rules, expected proof. |
| High-risk | ~10K tokens | Full intake, relevant decisions, high-risk templates, the architecture/wiring knowledge docs, and the proof plan before implementation. |

Budget rules:

- Prefer targeted search (`grep`, a code-graph tool when one is registered in
  `docs/TOOL_REGISTRY.md`) over bulk reading.
- Read the smallest section that answers the current phase question.
- Escalate context when a retrieval trigger fires.
- Stop reading history once the lane, affected files, and validation path are
  clear.
- The documentation phase reuses context the task already has. Re-read only the
  template, the diff, and the docs the change may have invalidated — not the
  knowledge base again. Budget it inside the lane's figure above, not on top.
