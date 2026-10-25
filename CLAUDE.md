# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

unity-template is the team's **shared Unity starting project**: the project a new
Unity game is created from, not a shipped game (`.claude/knowledge/01-project-brief.md`).
A change here reaches every project created from it afterwards.

It was cut from an earlier game (Jigsaw), but the Jigsaw runtime, content, and
internal package consumers have been removed. The live baseline is deliberately
small: DT/DA primitives in `Assets/game.core/`, a neutral boot owner in
`Assets/game.bootstrap/`, reusable shell UI in `Assets/game.ui/`, and five
Addressable prefabs under `Assets/game.assetbundles/{00_local,01_init}`. A real
feature creates `02_partial/<feature>` when it arrives; the template does not
carry placeholder business modules.

## Build & Development

- **Unity Version:** 6000.3.10f1
- **Render Pipeline:** URP (Universal Render Pipeline) 17.3.0, with `com.unity.feature.2d`
- **Target Platform:** not decided yet; no saved platform build profile exists.
- **Clone:** a normal clone is complete; there are no submodules or private
  package prerequisites.
- **Proof:** compile clean, all EditMode tests, and the PlayMode boot from
  `Assets/game.contents/gameScenes/00_LoadingScene.unity` to
  `TemplateBootState.Ready`. Run them through the Unity CLI commands in
  `.claude/knowledge/03-build-and-profiles.md`; there is no Unity MCP (decision
  `0007`).
- **Addressables build:** Editor menu `Game Template/Addressables/Build Content`
  or `TemplateAddressablesSetup.BuildContent`; audit with
  `tools/addressables/`.
- **CI / player build script:** none yet; content build has one batch-callable
  entry, while the player build argument contract remains planned.

### Scripting Define Symbols

- No template-owned runtime path depends on a project profile, Firebase,
  AppsFlyer, authentication, ads, or backend define.
- Product SDK/profile symbols are absent. Retained non-service plugins can still
  maintain capability symbols until their separate consumer-and-value audit.
- `UNITY_EDITOR` gates Editor-only tooling and tests.

Full list and where each is set: `.claude/knowledge/03-build-and-profiles.md`.

## Architecture

### Initialization Flow

1. `00_LoadingScene` contains the template-owned `TemplateBootstrap`.
2. The scene-wired loading overlay is available before Addressables or global DI.
3. `TemplateBootstrap` instantiates `P_GlobalUiShell` through its serialized
   `AssetReference` and owns the returned handle.
4. Success hides loading and publishes `TemplateBootState.Ready`; failure keeps
   the local recovery surface visible.
5. Destroying the bootstrap releases the instantiated shell handle.

Detail: `.claude/knowledge/02-runtime-and-wiring.md`.

### Layer Structure (top → bottom)

| Layer | Assembly | Location |
|-------|----------|----------|
| Neutral bootstrap | `game.bootstrap.runtime` | `Assets/game.bootstrap/Runtime/` |
| Reusable shell UI | `game.ui.runtime` | `Assets/game.ui/Runtime/` |
| Thin data core | `game.core.runtime` | `Assets/game.core/Runtime/` |
| Addressables setup | `game.bootstrap.addressables.editor` | `Assets/game.bootstrap/Editor/Addressables/` |
| Project identity setup | `game.bootstrap.project.editor` | `Assets/game.bootstrap/Editor/Project/` |
| Tests | `game.core.tests.editmode`, `game.bootstrap.tests.*` | `Assets/game.core/Tests/`, `Assets/game.bootstrap/Tests/` |

Direction: bootstrap may use UI and Unity Addressables; UI and core do not
depend on bootstrap or any internal package. The boundary test rejects retained
runtime/editor assemblies that reference `com.paul.*` or `Paul.*`.

### Optional Git UPM capabilities

The template ships no internal framework, networking, auth, backend, or VFX
package. A derived project adds only the package its first consumer needs,
pinned to a commit SHA in the project manifest. Read
`docs/architecture/networking-on-demand.md`; Git dependencies required by
another Git package must also be declared directly by the project (decision
`0008`).

### Key Patterns

**UI shell:** local loading/recovery prefabs are concrete boot surfaces;
post-init Toast and MessageOverlay live in the global shell. Feature screens and
popups belong to their future `02_partial/<feature>` module. Rules:
`.claude/rules/ui-code.md`.

**Composition and services:** R3 remains for the neutral reactive shell. The
template has no DI container, async helper package, global application scope,
or service facade. A derived feature installs and owns those capabilities with
its first real consumer.

**Async:** the template does not prescribe UniTask. Thread
`CancellationToken` through async feature boundaries and make fire-and-forget
failures observable using the package or runtime abstraction that project has
explicitly chosen.

**Data:** `DataTable<TRow>` / `DataAsset<TData>` ScriptableObjects
(`docs/architecture/template-core-data.md`). The template includes primitives,
not game master data or persistence.

**Addressables:** SmartAddresser owns membership for exactly `game.local` and
`game.init`; `TemplateAddressablesSetup` is the idempotent setup/build entry.
The object that loads or instantiates a handle owns its release.

### Key Entry Points

- `Assets/game.bootstrap/Runtime/TemplateBootstrap.cs` — neutral boot owner
- `Assets/game.bootstrap/Editor/Addressables/TemplateAddressablesSetup.cs` — layout/build entry
- `Assets/game.bootstrap/Editor/Project/TemplateProjectSettingsSetup.cs` — neutral identity entry
- `Assets/game.ui/Runtime/` — shell UI views
- `Assets/game.core/Runtime/Data/` — DT/DA core contract

## Key Dependencies

- **R3** + its four-DLL NuGet closure — reactive extensions (Cysharp)
- **Addressables** 2.9.0 + **SmartAddresser** — content delivery and layout rules
- **URP** 17.3.0 with its 2D renderer
- **TextMesh Pro** + **uGUI** 2.0.0
- **Unity Test Framework** 1.6.0
- **NuGetForUnity** — manages the retained R3 binary dependency set

No Firebase, attribution, ads, IAP, Unity cloud-service, or mobile resolver SDK
is bundled. A derived project adds one only with its first real consumer
(decision `0010`).

No vendored art pack, sample, runtime plugin, or editor tool is kept without a
live template consumer. Derived projects install those capabilities on demand
(decision `0011`).

The direct non-module package baseline is exactly the seven capabilities above;
optional gameplay/editor packages arrive with their first consumer (decision
`0012`).

## AI Harness

The AI harness lives in-repo under `.claude/` and `docs/`, and follows
cricket-city-unity's (decision `0007`). See `docs/HARNESS.md` for the operating
model and `.claude/ONBOARDING.md` for orientation.

- `.claude/skills/unity-task-router/` — start every Unity task here; classifies
  the task and routes to the cheapest correct path.
- `.claude/agents/` — specialist subagents: `unity-game-logic-developer`,
  `unity-code-reviewer`, `unity-documenter`. There is no UI agent: UI is uGUI,
  built inline (decision `0007`). Design runs on the built-in `Plan` agent with
  Fable (decision `0002`).
- `.claude/skills/feature-domain-service/` — the server-feature stack, top down:
  how a backend feature becomes its own service. Read this before adding a
  service or splitting one out of `AppServices`.
- `.claude/skills/gateway-wiring/`, `.claude/skills/api-codegen/` (config:
  `.claude/api-codegen-config.md`), `.claude/skills/unity-editor-scripting/`
  — the two layers below it (gateway → DI, then one endpoint's classes after a
  derived project configures its on-demand networking package), and
  editor-automation mechanics.
- `.claude/skills/bundle-optimization/` — audit and cut Addressables download
  sizes with `tools/addressables/`; start here for any "why does this download"
  or "can this group be excluded" question.
- `.claude/skills/tmp-sprite-number-sheet/` — TMP sprite number sheets.
- `.claude/skills/epic-planning/` — one feature epic: spec → a local preview the
  owner approves → the epic and its Unity tickets in Jira. Its team values are
  `TODO(project)` until a game fills them. User-requested only.
- `.claude/knowledge/` — codebase knowledge base (`00-Master-Index.md` first).
- `.claude/rules/` — **rules for the code you write**, gates unless the file says
  otherwise: `csharp-code-style.md`, `csharp-type-choice.md` (a guideline),
  `ui-code.md`, `asset-structure.md`, `code-comments-and-naming.md`,
  `docs-writing.md`. Each carries `paths:` frontmatter, so Claude Code loads it
  automatically once a matching file is read; a subagent or non-Claude agent reads
  the matching file before writing C#, not after review (decision `0004`).
  `.agents/` and `.agent/` do not exist — do not recreate them.
- `.claude/skills/onboard-repository/` + `.claude/skills/audit-onboarding-proposal/`
  — user-invoked only. Map one real operational path and propose evidence-backed
  guidance patches, then audit those patches in a separate session before any are
  applied (decision `0006`).
- `.claude/skills/encode-invariant/` — turn an accepted rule (in `CLAUDE.md`,
  `.claude/rules/`, or a decision) into a mechanical check with positive and
  negative proof; the method is `docs/patterns/encoding-invariants.md`.
- `.claude/skills/harness-lifecycle/` — harness status, tuning, shipping;
  `learning-distillation`, `harvest`, `harness-bootstrap` maintain it.
- `docs/FEATURE_INTAKE.md` / `docs/CONTEXT_RULES.md` — risk lanes and per-lane
  reading rules.
- `docs/decisions/` — durable rulings. Worth knowing before you start:
  - `0002` — role-based dispatch: which model runs which role, and the Fable
    design-consult rule.
  - `0003` — this repo runs the harness only; the superpowers plugin is
    disabled in-project.
  - `0007` — the harness follows cricket-city-unity: names, no UI agent, no
    Unity MCP. Do not re-run the framework installer against this repo.

Docs — stories, `docs/decisions/`, plans — are written on the branch of the work
they belong to. There is no standing planning branch (decision `0005`).

### Index Tools: Check Freshness Before Trusting Them

`codebase-memory` answers code-discovery questions faster than grep — and fails
*silently* when stale: a plausible answer that is simply not about this branch.
**Check `index_status` before the first query of a session**, and re-index
(`index_repository`) when it predates the branch's work.

| Question | Use |
| --- | --- |
| What C# exists, how is it shaped | `codebase-memory` `search_graph` |
| Who calls this | `trace_path`, **then confirm with grep** |
| What references this asset/prefab/scene | grep the asset's GUID from its `.meta` across `Assets/`, or `AssetDatabase` in the Editor / Unity CLI |
| What is *inside* a scene or prefab instance | Read/Grep the YAML |

Limits measured in cricket-city-unity on the same tool (2026-08-25):

- **`trace_path` misses `?.` call sites.** A caller written `_core?.Method(...)`
  does not show up; a plain field call does.
- **A bare name declared on an interface returns `callers: []`.** The query binds
  to the interface node, which nothing calls, and does not report the ambiguity.
  Never conclude "nothing calls this" from `trace_path` alone.
- **`search_code` only sees matches inside a function body.** A match in an enum,
  class, or field declaration is dropped, and `total_results` counts functions,
  not files, so the drop never looks like truncation. `search_graph` does hold
  those declarations.

The same order applies everywhere: the index first, grep to confirm. A grep miss
on a GUID is not proof an asset is unused; check the `.meta` and the Editor.

General Unity C# uses the user-level `unity-developer` skill.

## Harness

Claude Code loads this file into every session, but it does not auto-load
`AGENTS.md`. The bare `@` lines below import the always-required harness
context (the "Must in all lanes" set from `docs/CONTEXT_RULES.md`) at
context-load time. Never wrap them in backticks; that disables the import.

@AGENTS.md

@docs/FEATURE_INTAKE.md

Start Unity tasks with the `unity-task-router` skill
(`.claude/skills/unity-task-router`), then read the proof table at the top of
`docs/TEST_MATRIX.md` and grep it for the rows covering the area you will change —
the file is too large to read whole.

**Markdown is the only source of truth.** There is no database and no CLI — the
files in `docs/` are the record. Before the first code write, classify the work
per `docs/FEATURE_INTAKE.md`, emit the lane block to the user, and append a row
to `docs/INTAKE_LOG.md`. A `PreToolUse` hook blocks the first code write when no
recent intake is logged; docs and markdown are never gated.

On completion, write what the lane owes — the story packet, its
`docs/TEST_MATRIX.md` proof row, a `docs/decisions/NNNN-*.md` record when
architecture or a contract changed, and a `docs/TRACE_LOG.md` entry citing the
intake timestamp. See `docs/contracts/harness-artifacts-v1.md` for where each
artifact lives. A `Stop` hook reminds once when normal or high-risk work has no
trace.

Lane-dependent context (`README.md`, `docs/HARNESS.md`, `docs/ARCHITECTURE.md`,
`docs/CONTEXT_RULES.md`, product docs, stories, decisions) is intentionally not
imported — read it per lane, as `docs/CONTEXT_RULES.md` prescribes.
