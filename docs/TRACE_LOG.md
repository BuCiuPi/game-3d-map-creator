# Trace Log

What actually happened during tasks. Format and depth rules live in
`docs/TRACE_SPEC.md`; the short version:

- Normal and high-risk work gets a row. Tiny work gets one only if it hit
  friction or changed harness rules.
- High-risk work adds a `### YYYY-MM-DD — Task` section below the table.
- Newest first. Rows are not edited after the fact — this is a log. A correction
  is a new row.

The **Friction** column is the point of this file. `docs/IMPROVEMENT_PROTOCOL.md`
reads it to decide what the harness should fix next.

## How To Add A Row

Add a row at the **top** of the table:

| Column | Value |
| --- | --- |
| Date (UTC) | `YYYY-MM-DD` |
| Intake | The `docs/INTAKE_LOG.md` stamp(s) this trace closes, `YYYY-MM-DD HH:MM`, comma-separated; `—` when there is none |
| Task | One line naming the outcome; link the story packet when there is one |
| Lane | `tiny` · `normal` · `high-risk` |
| Outcome | `completed` · `partial` · `blocked` · `failed`, plus the proof actually run |
| Changed | Paths or an area name, or a commit sha |
| Friction | The concrete pain, or a deliberate `none` |

The `Stop` hook `.claude/hooks/trace-gate.js` treats an intake as traced when its
stamp string appears anywhere in this file, so copy the stamp exactly.

`.gitattributes` marks this file `merge=union`, so branches that each add rows
merge without conflicts. After a merge, check that the table is still
newest-first and that no row was duplicated.

## Log

| Date (UTC) | Intake | Task | Lane | Outcome | Changed | Friction |
| --- | --- | --- | --- | --- | --- | --- |
| 2026-10-26 | 2026-10-25 16:50 | [Asset prefab inspector](stories/asset-prefab-inspector.md): render and measure every prefab of a pack, one doc card each | normal | Completed. Batch run exit 0, 238 prefabs / 238 PNGs, 0 `error CS`; bad path exit 1; review PASS WITH WARNINGS, all three warnings fixed (empty-result guard, sync shader compile, shared LOD0 renderers). | `Assets/game.assetdocs/Editor/`, `tools/assets/gen_prefab_tables.py`, `docs/assets/` | Unity deletes `Temp/` on exit, so the first metrics file vanished; metrics now go to `Logs/`. The Editor held the project open with unsaved changes, so batch mode needed the owner's go-ahead to kill it. Previews contradicted names: `RiverStraight` pieces are banks with no water. |
| 2026-10-25 | 2026-10-25 11:49 | [Package baseline prune C8.2B](stories/package-baseline-prune/overview.md): retain only packages with a live template or dependency-management role | high-risk | Completed. Manifest 31 → 7 direct non-module packages; lock 94 → 55; setup/compile exit 0; EditMode 12/12; PlayMode 1/1; Addressables 2/2 groups byte-identical. Normal clone of `9423722` imported/compiled in 137.8 s, passed EditMode 12/12, kept lock/roots stable, and ended content-clean. | Product `9423722`; removed 24 direct package declarations, 39 lock nodes, 11.70 MiB unused NuGet payload, Coffee settings/preloads; added input/package invariants and decision `0012` | Intake used the still-current C8.2 high-risk window during implementation, but this independently locked package cohort deserved its own row and received one before closeout. Unity's Windows launcher returned before the background Editor worker finished, so fresh-clone proof waited for process/log completion rather than trusting the immediate shell exit. |
| 2026-10-25 | 2026-10-25 10:32 | [Vendored asset prune C8.2A](stories/vendored-asset-prune/overview.md): remove every zero-consumer vendored art/editor/runtime root | high-risk | Completed. 0 retained C#/GUID consumers; compile/setup exit 0; EditMode 12/12; PlayMode 1/1; Addressables 2/2 groups byte-identical. Normal clone of `7475d9b` compiled in 261.3 s, passed EditMode 12/12, retained 0 roots/defines after import, and kept the lockfile stable. | Removed 9,147 files / 581.03 MiB across 8 roots; cleared DOTween/Odin defines; added decision `0011`, boundary invariant, inventory and proof | Retained Rider logs a missing local EditorPlugin path, and Coffee/Code Coverage/ShaderGraph still rewrite settings in a disposable fresh clone. These are isolated for C8.2B package audit. |
| 2026-10-25 | 2026-10-25 09:54 | Service SDK prune C8.1 correction: remove two service settings missed by the root-based cohort | high-risk | Completed. Repository scan found no remaining active service marker; EditMode passed 12/12 with 0 compile/package error markers. | `275afad`: removed Unity IAP `BillingMode.json` and `AppLovinInternalSettings.json`; extended the file-absence invariant | The first inventory was root-oriented and did not classify two standalone settings. C8.2 now starts with both root and repository-wide filename/content scans. |
| 2026-10-25 | 2026-10-25 09:54 | [Service SDK prune C8.1](stories/service-sdk-prune/overview.md): remove the zero-consumer mobile service cohort and make provider capability demand-driven | high-risk | Completed. Current worktree compile exit 0, EditMode 12/12, PlayMode 1/1; Addressables 2/2 groups byte-identical. Normal clone of `20c4f30` compiled with exit 0, passed EditMode 12/12, kept the lockfile stable, and retained 0 service roots/markers after import. Platform deferred. | Removed Firebase, EDM4U, generated Maven/native mobile/service Android roots, 4 Unity service packages, 2 registry scopes and resolver settings; added decision `0010`, boundary invariant and story proof | Unity fresh import still creates known changes for retained Spine/Odin/SoftMask/ShaderGraph assets in the disposable clone; they do not affect the source worktree. The full import remained dominated by retained art/editor tooling, which is the separate C8.2 audit. |
| 2026-10-25 | 2026-10-25 09:35, 2026-10-25 09:36 | [Template identity C7](stories/template-identity/overview.md): replace Jigsaw identity with a reproducible neutral baseline and remove product build/service config | high-risk | Completed. Compile exit 0; EditMode 12/12; PlayMode 1/1; identity setup idempotent by SHA-256; legacy scan 0 matches; Addressables report 2/2 groups byte-identical. Platform deferred. | Player settings, project Editor setup, boundary tests, Android profiles, two Firebase app-config files, decision `0009`, knowledge and proof docs | Unity 6 retains obsolete `BuildTargetGroup.WebPlayer` but rejects its `NamedBuildTarget`; enumeration now skips invalid legacy targets. Retained plugins automatically re-added capability symbols, so C7 enforces only product SDK/profile symbols and leaves plugin removal to C8. Known Odin/SoftMask/URP/content-state import side effects were restored. |
| 2026-10-25 | 2026-10-25 05:14 | Package exit C6: finalize the thin-template handoff, provider-neutral skills, and reproducible Addressables baseline | high-risk | Completed. Compile exit 0; EditMode 10/10; PlayMode 1/1; static/report checks 0 errors/warnings; report `13.39.02` has 2 groups, 3 bundles, 0.02 MB and is byte-identical 2/2 vs `13.03.10`. | Current architecture/onboarding/knowledge/rules/agents/API skills; networking-on-demand recipe; both local plan artifacts; package-exit proof docs | PowerShell initially dropped quotes around the project path for compile/test launches; rerunning with explicitly quoted argument variables worked. Unity regenerated the known Odin/SoftMask/URP/settings side effects; all were restored or removed. Platform/HAR intentionally deferred. |
| 2026-10-25 | 2026-10-25 05:14 | Package exit C5: remove internal package dependencies and both submodule gitlinks | high-risk | Completed. The regenerated lockfile has no `com.paul.*` or local submodule path. A fresh non-recursive clone of `59a0465` had no `.gitmodules`, package directories, or submodule output; Unity exited 0 with no compile errors and EditMode 10/10. | Deleted `.gitmodules` and both gitlinks; removed 7 manifest/lock dependencies; updated package-exit plan and proof docs | The first current-worktree launch reused stale UPM state, so the old lockfile was removed and regenerated before testing. The truly fresh clone required a full asset/shader import; the Feel toon shader dominated the run but completed successfully. |
| 2026-10-25 | 2026-10-25 05:14 | Package exit C4: retire Jigsaw and package/VFX consumers, then rebuild the two-tier Addressables layout | high-risk | Completed. Package GUID scan: 1,423 scanned / 0 references under `Assets/`. Addressables: 2 groups, 3 bundles, 0 errors/warnings; two layout applications were byte-identical and two fresh reports had identical group sizes. Unity compile exited 0; EditMode 10/10; PlayMode 1/1. | Removed Jigsaw roots and legacy groups; added template-owned Addressables setup under `Assets/game.bootstrap/Editor/Addressables/`; updated check/build-layout conventions, plan, and proof docs | The historical C2 build could not produce a pre-removal report, so the first valid report is intentionally C4. Unity regenerated unrelated Odin/SoftMask/VFX files during validation; they were restored or removed before commit. Rider 2025.3.3 EditorPlugin remains absent but did not affect compile or tests. |
| 2026-10-25 | 2026-10-25 05:14 | Package exit C3: replace Jigsaw startup with a neutral Addressables-backed bootstrap and reusable UI shell | high-risk | Completed. Unity compile exited 0; EditMode 10/10; PlayMode boot 1/1; Addressables static check 0 errors with 2 `game.local` and 3 `game.init` entries. | `Assets/game.bootstrap/`, `Assets/game.ui/`, neutral loading scene, five shell prefabs, template SmartAddresser rules/groups, plan and proof docs | PowerShell initially split the project path at its space; rerun with quoted arguments passed. Unity repeatedly regenerated unrelated SoftMask/Odin/VFX files, which were removed/restored. PlayMode emitted a Bee IPC shutdown message after the test runner had already completed with code 0. |
| 2026-10-25 | 2026-10-25 05:14 | Package exit C2: freeze the package assembly, C#, serialized GUID, and Addressables disposition map | high-risk | Dependency ownership gate passed: 18 assembly refs, 87 C# files, and 1,104 serialized occurrences from 56 source files to 311 package GUIDs were assigned `KEEP-REWRITE`, `DELETE-LEGACY`, or `PROJECT-ADDON`. Static Addressables check exited 0 with 495 warnings. | `docs/stories/package-exit/dependency-inventory.md`, plan and proof logs; no product/package/asset mutation | Fresh Addressables build produced no report: package entry `Square.shadergraph` in `com.vfx.third_party` cannot import. The whole group is a known `PROJECT-ADDON`; report/diff is deferred until its C4 removal. The temporary build helper and Unity import side effects were removed/restored. |
| 2026-10-25 | 2026-10-25 05:14 | Package exit C1: establish a green Unity compile baseline before changing consumers or submodules | high-risk | Corrected `wnamespace` to `namespace` in legacy `PopupNoAdsData.cs`. Unity 6000.3.10f1 batch compile exited 0; no `error CS`; script compilation 2.46 s after a 376.27 s full import. No package, `.asmdef`, asset, GUID, or submodule changed. | `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Monetization/NoAds/PopupNoAdsData.cs`, package-exit plan/story/decision and proof logs | Unity reports `.agent-runtime` as an invalid project-root directory name; the full import also rewrote settings, deleted four Odin demo `.meta` files and created a duplicate SoftMask settings asset. All import side effects were restored or removed before the changeset. |
| 2026-10-25 | 2026-10-25 03:59 | Pre-ship validation of the Cricket-aligned harness and package state | high-risk | Harness checks passed: JSON parsed, hooks passed `node --check`, six Addressables scripts parsed, and `check.py --static` exited 0 with 0 errors / 485 warnings. Unity 6000.3.10f1 resolved packages and reached Roslyn; compile failed only in legacy Jigsaw `PopupNoAdsData.cs` because line 1 starts with `wnamespace` (CS1513, CS1529, CS1022). | `.claude/knowledge/04-port-map-from-cricket.md`, `docs/decisions/0007-harness-follows-cricket-city.md`, `docs/TEST_MATRIX.md`, `docs/TRACE_LOG.md` | A fresh checkout requires both private submodules before package resolution; their checkout dominated validation time. Unity import also removed four Odin demo `.meta` files, which were restored byte-for-byte from `HEAD` and excluded from the change. |
| 2026-10-25 | 2026-10-25 03:59 | Tune the template harness to follow cricket-city-unity @ 8da403c97 (decision `0007`): router `unity-task-router` with the Opus/Fable role table; agents `unity-game-logic-developer`, `unity-code-reviewer`, `unity-documenter` (7 framework agents retired, no UI agent); skills `feature-domain-service`, `gateway-wiring`, `api-codegen`, `bundle-optimization`, `tmp-sprite-number-sheet`, `epic-planning`, deeper `unity-editor-scripting`; rules `csharp-code-style`, `csharp-type-choice`, `ui-code`, `asset-structure`; `tools/addressables/`; decisions `0001`–`0007`; Unity MCP package removed | high-risk | completed (harness only): strict-YAML frontmatter on 15 skills and 3 agents; every skill and agent named in the router; `node --check` on both hooks; 4 JSON files parse; `py_compile` on 7 scripts; `check.py --static` exit 0 (0 errors, 485 warnings); report tools run read-only on another project's Addressables 2.9.0 reports; grep: no old names or Unity MCP tools outside history and the framework's `harness-bootstrap`. Not run: compile, EditMode, boot (submodules not checked out; `com.paul.common.networking` not mounted). review: skipped — harness docs and tools, no product code | `.claude/`, `docs/`, `CLAUDE.md`, `AGENTS.md`, `tools/addressables/`, `Packages/manifest.json`, `Packages/packages-lock.json` | `install_harness.ps1` re-adds every missing managed file, so a reinstall would bring the retired agents, skills and rules back; `.claude/harness-manifest.json` is now stale. The framework's `harness-bootstrap` still names the generic roles. The template manifest lacks `com.paul.common.networking`, which `com.paul.game.shared.runtime` references |
| 2026-10-24 | 2026-10-24 18:40 | Harness init (core + unity-client, framework 833b45a) and bootstrap of the shared Unity template: router `unity-template-task-router`, knowledge 01 brief, 02 runtime and wiring, 03 build and profiles, placeholders filled from the code, `guardedRoots` Assets/Packages/ProjectSettings, `.gitignore` merge | normal | completed: installer re-plan 0 changes; `node --check` on both hooks; 3 JSON configs parse; strict-YAML frontmatter on every skill and agent; every skill and agent reachable from the router; inventory superset; 8 allowed placeholders, 14 explained TODOs. Not run: compile, EditMode tests, boot check (package submodules not checked out in this clone). review: skipped — harness docs only, no product code | `.claude/`, `docs/` (harness files), `CLAUDE.md`, `AGENTS.md`, `Learnings.md`, `.gitattributes`, `.gitignore` | Submodules `Packages/game-packages` and `Packages/vfx-packages` are empty in this clone, so no compile proof was possible. The Unity agents list `mcp__mcp-unity__*` tools but the template ships MCP for Unity (`com.coplaydev.unity-mcp`) and no `.mcp.json`. A game created from this template inherits this harness, router name and bootstrap record; the framework has no step for re-bootstrapping a template-derived repository |

### 2026-10-25 — Package baseline prune C8.2B

- Proved zero retained code/GUID consumers for 24 direct dependency candidates,
  including the complete `com.unity.2d.*` feature dependency set.
- Reduced the manifest to seven direct non-module capabilities and regenerated
  the lock from 94 to 55 nodes without adding a dependency.
- Reduced the NuGet payload to the four assemblies required by R3; removed
  Coffee settings/preloads and aligned Active Input Handling with the retained
  legacy Input Manager.
- Added repeatable input setup plus package/root/preload boundary guards.
  Compile/setup, EditMode 12/12, PlayMode 1/1, and a byte-identical two-group
  Addressables build passed.
- A normal clone of `9423722` completed full import/compile in 137.8 seconds,
  passed EditMode 12/12, and ended with a stable lock and clean content diff.

### 2026-10-25 — Vendored asset prune C8.2A

- Mapped 15 subcohorts, 5,078 owned GUIDs, and all eight root GUIDs before
  deletion. No game-owned code, serialized asset, Addressables setting, or
  project setting required the cohort; DOTween and Spine only referenced their
  own settings, which were removed with them.
- Removed 9,147 tracked files / 581.03 MiB from `Assets/UnityAssetStore`,
  `AlmostEngine`, `StompyRobot`, `Plugins`, `Spine`, `UnitySkipSplashScreen`,
  `Editor`, and `Resources`, including each root `.meta`.
- Extended the repeatable settings setup and boundary test to reject the eight
  roots plus DOTween/Odin symbols. Compile, EditMode 12/12, PlayMode 1/1, and
  Addressables static/build/report diff all passed.
- A normal clone of `7475d9b` compiled in 261.3 seconds versus the earlier
  376-second full-import baseline, passed EditMode 12/12, and did not regenerate
  a removed root, define, or lockfile delta.

### 2026-10-25 — Service SDK prune C8.1

- Removed the complete zero-consumer service cohort: seven Firebase, resolver,
  generated-repository, and native mobile roots plus their project settings,
  custom Android template state, four Unity service packages, and two service
  registry scopes.
- Added an EditMode invariant over service roots, manifest, lockfile, package
  manager settings, resolver files, and Android custom-template flags.
- Proved compile, EditMode 12/12, PlayMode 1/1, and a byte-identical two-group
  Addressables build in the source worktree.
- Cloned product commit `20c4f30` normally. Its clean package graph compiled,
  passed EditMode 12/12, left the lockfile unchanged, and did not regenerate a
  removed service root or marker.

### 2026-10-25 — Template identity C7

- Added `TemplateProjectSettingsSetup` as the permanent menu/batch entry for
  neutral company, product, identifiers, versions, and product-define cleanup.
- Removed the unreferenced Android build-profile assets plus Jigsaw Firebase app
  configuration. SDK implementations remain isolated for C8.
- Encoded two EditMode invariants: the exact neutral identity, and absence of
  saved product build/service config and product SDK/profile symbols.
- Proved compile, EditMode 12/12, PlayMode 1/1, setup idempotence, legacy-string
  absence, and a byte-identical Addressables build.

### 2026-10-25 — Package exit C6 thin-template handoff

- Rewrote live architecture, onboarding, knowledge, rules, and specialist
  guidance around the neutral `game.bootstrap` / `game.ui` / `game.core`
  surface. Historical Cricket/package instructions no longer act as live
  contracts.
- Made API generation demand-driven: the skills stop while
  `.claude/api-codegen-config.md` is unconfigured, and the derived project must
  mount a direct Git UPM dependency pinned to a commit before code generation.
- Rebaselined the Addressables plan to two local groups and a first-feature-only
  `02_partial` tier. P3-P7 remain demand-driven.
- Re-ran Unity compile, EditMode 10/10, PlayMode 1/1, Addressables static/build,
  check, audit, and diff. The fresh report has 2 groups / 3 bundles / 0.02 MB;
  both groups are byte-identical to the previous valid baseline.

### 2026-10-25 — Package exit C5 package and gitlink removal

- Removed all seven `file:` dependencies backed by the two internal package
  repositories. Unity regenerated `packages-lock.json` without a `com.paul.*`,
  `game-packages`, or `vfx-packages` reference.
- Deleted both gitlinks and `.gitmodules`; the retained assemblies contain no
  internal package reference outside the boundary test's forbidden-name data.
- Cloned product commit `59a0465` normally, without recursive submodules. The
  clone contained no submodule metadata or package directories and reported no
  submodule status.
- Unity `6000.3.10f1` performed a clean package resolve and full import in the
  fresh clone, exited 0 with no C# compilation error, and passed EditMode 10/10.

### 2026-10-25 — Package exit C4 legacy consumer retirement

- Removed the Jigsaw art, gameplay, shared, visualization, lobby, data, sound,
  effects, and UI roots as complete vertical slices. The preserved loading scene
  still boots the template-owned C3 shell.
- Replaced the package-backed Addressables configuration with the permanent
  `TemplateAddressablesSetup`: only `game.local` and `game.init` remain, both
  use local build/load paths, and a second application produced byte-identical
  settings, groups, schemas, and SmartAddresser rule data.
- Scanned all 1,423 GUIDs owned by the two pinned packages and found no remaining
  serialized reference under `Assets/`. Package-owned group assets were only
  detached from this project's settings; neither submodule was edited.
- Fresh report checks passed with 2 groups, 3 bundles, 0 errors, and 0 warnings.
  Unity compile, EditMode 10/10, and PlayMode boot 1/1 also passed after removal.

### 2026-10-25 — Package exit C3 neutral bootstrap and UI shell

- Added template-owned `game.bootstrap` and `game.ui` runtime assemblies with no
  `com.paul.*` assembly reference. The bootstrap owns and releases the
  Addressables instantiation handle for the global shell.
- Replaced the loading scene contents while preserving its scene GUID. Legacy
  Jigsaw runtime initializers are disabled pending their C4 deletion, so the
  neutral bootstrap is the only startup owner.
- Generated five reusable uGUI/TMP prefabs under the agreed tier root:
  loading/recovery in `00_local/ui/shell`, and Toast/MessageOverlay/global shell
  in `01_init/ui/00_shared`.
- Added SmartAddresser rules for `game.local` and `game.init`. Compile, all 10
  EditMode tests, the PlayMode boot test, and the static Addressables check
  passed. The 495 warnings belong to legacy/package entries scheduled for C4.

### 2026-10-25 — Package exit C2 dependency inventory

- Indexed both pinned submodules and matched all package `.meta` GUIDs against
  serialized `guid`, `m_GUID`, and `m_AssetGUID` fields under `Assets/`.
- Assigned every C# and serialized consumer to `KEEP-REWRITE`,
  `DELETE-LEGACY`, or `PROJECT-ADDON`; the full reproducible map is
  `docs/stories/package-exit/dependency-inventory.md`.
- Used `bundle-optimization`: static check passed with no errors. A temporary
  Editor batch helper enabled Addressables build-layout generation and called
  `AddressableAssetSettings.BuildPlayerContent`; it was deleted after the run.
- The build stopped at a known VFX package entry before producing a report. No
  package source or Addressables YAML was edited to bypass it. Unity-generated
  settings/meta side effects were restored before recording the inventory.

### 2026-10-25 — Template harness follows cricket-city-unity

- Owner answers (2026-10-25): cricket names and content; UI inline, no UI agent; no Unity MCP, Unity CLI instead; port the service/gateway stack, `epic-planning`, the bundle tools and the harness decisions. Recorded in `docs/decisions/0007-harness-follows-cricket-city.md`; port map in `.claude/knowledge/04-port-map-from-cricket.md`.
- Four workers ported in parallel (agents; service/gateway/api-codegen; bundle tools and asset skills; rules and `epic-planning`); routing, registry, matrix and verification done in the main session.
- Open for the owner: `epic-planning` and `api-codegen-config` `TODO(project)` values; `EXPECTED_BOOT` groups; Addressables setup drift on disk; plan P5 says not to copy Cricket's tool source.
- Owner answers, same day: the owner picks the Unity CLI; the API gateway maps to the model (`Project`), as cricket; shared folders are `00_shared`. Recorded in `0007`; skills, rule and `check.py` updated, `check.py --static` still exit 0.
- Owner, same day: the template does not mount `com.paul.common.networking`; a game mounts it with its first endpoint. Recorded in `0007`, `CLAUDE.md`, the brief and the three API skills.
