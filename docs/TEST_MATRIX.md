# Test Matrix

This file maps behavior to proof. Do not mark a row proven until the evidence
exists.

Read the Proof Types table, then grep the rows for your area. Do not read the
whole matrix once it grows.

## Proof Types

| Proof | Covers | How |
| --- | --- | --- |
| `compile` | every C# change | Editor recompile with a clean console, or batch mode: `Unity.exe -batchmode -quit -projectPath . -logFile -` with no `error CS`; a normal clone needs no submodule step |
| `editmode` | `Assets/game.core/` data primitives, template boundaries, and plain C# logic | `Unity.exe -batchmode -projectPath . -runTests -testPlatform EditMode -testResults .agent-runtime/editmode.xml -logFile -`, or the Test Runner (current assemblies: `game.core.tests.editmode`, `game.bootstrap.tests.editmode`) |
| `boot` | bootstrap, lifetime scope, Addressables init, scene routing | Run the PlayMode boot test or play `Assets/game.contents/gameScenes/00_LoadingScene.unity`; it reaches the neutral global UI shell with no boot error |
| `editor-evidence` | UI screens, popups, prefab wiring, Addressables menu passes | run in the Editor and capture a screenshot or the console; the owner judges the look |
| `addressables-build` | group, profile, layout-rule or asset-move changes | Run `Game.Bootstrap.Editor.Addressables.TemplateAddressablesSetup.BuildContent` in Unity batch mode, then `python -I tools/addressables/check.py` exits 0 on the new report, and `audit.py` plus `diff.py` against the previous report show the size change (`bundle-optimization`) |
| `addressables-static` | any change under `Assets/AddressableAssetsData/` or to addressable assets, before a build exists | `python -I tools/addressables/check.py --static` exits 0 with no errors (warnings are reported, not failures) |
| `platform` | a device build of a game made from the template | deferred to the owner; the main platform is an open question |

## Matrix

Add a row per normal/high-risk story, newest first. Keep it current as proof
lands. Rows are edited in place as status changes.

`.gitattributes` marks this file `merge=union`, so branches that each add rows
merge without conflicts. A row edited on both branches comes out of the merge
twice — after any merge that touches this file, keep the newer version of each
duplicated row and delete the other.

| Column | Value |
| --- | --- |
| ID | The story id or slug — the story packet's folder or file name, so `docs/HARNESS_AUDIT.md` can match it |
| Area | The codebase area or feature |
| Behavior | The behavior being proven, in one or two sentences |
| Proof | The proof types required, by the names in the Proof Types table |
| Status | A value from Status Values below |
| Evidence | What was actually run and its result (command, counts, link); `—` until it exists |

<!-- stack: if this repository's proof ladder is better tracked as one column per proof type (for example | Build | Unit | E2E |), replace the single Proof column with those columns, using the names from Proof Types. Keep ID, Area, Behavior, Status, and Evidence. -->

| ID | Area | Behavior | Proof | Status | Evidence |
| --- | --- | --- | --- | --- | --- |
| asset-prefab-inspector | Asset docs tooling (Editor-only) | `PrefabInspector.Run` renders and measures every prefab of a pack in batch mode; the generator writes one card per prefab with preview, bounds, pivot, triangles and colors; a bad pack path fails instead of emptying the cards. | compile, editor-evidence | implemented | 2026-10-26: batch run exit 0, 0 `error CS`, 238 prefabs, 238 PNGs; bad `-packRoot` exit 1 with no metrics file; 38 previews checked by eye (URP materials render); generator re-run gives no diff. |
| package-baseline-prune | Package and dependency boundary | A normal template clone declares only seven live non-module capabilities and the four-DLL R3 NuGet closure; optional packages, Coffee settings, and stale input configuration stay absent without changing boot or Addressables. | fresh-clone, compile, editmode, boot, addressables-static, addressables-build, platform | implemented | Product commit `9423722`: manifest 31 → 7 non-module direct dependencies; lock 94 → 55 packages (39 removed, 0 added); four unused NuGet roots / 11.70 MiB and Coffee settings/preloads removed. Setup/compile exited 0; EditMode 12/12; PlayMode 1/1; setup hashes stable. Report `buildlayout_2026.10.25.18.14.09.json` has 2 groups, 3 bundles, 0.02 MB and is byte-identical 2/2 vs C8.2A. A normal clone imported/compiled in 137.8 s with 0 error markers, lock/root invariants stable, EditMode 12/12, and content-clean status. Platform deferred because no target is selected. |
| vendored-asset-prune | Vendored asset/tool boundary | A normal template clone contains no zero-consumer vendored art pack, sample, animation/debug/runtime plugin, or editor utility; the live boot, UI, data, and Addressables contract remain unchanged. | fresh-clone, compile, editmode, boot, addressables-static, addressables-build | implemented | Pre-delete inventory at `bae1af6`: 9,147 files / 581.03 MiB across eight roots; graph found 0 template-owned C# consumers; 5,078 owned GUIDs and all eight root-folder GUIDs had 0 retained references. Product commit `7475d9b`: compile/setup exit 0; EditMode 12/12; PlayMode 1/1; 0 root/define residue. Addressables report `17.43.30` has 2 groups, 3 bundles, 0.02 MB and is byte-identical 2/2 vs C8.1. A normal clone compiled in 261.3 seconds with 0 error markers, passed EditMode 12/12, retained 0 roots/defines after import, and kept the lockfile unchanged. |
| service-sdk-prune | Mobile service dependency boundary | A normal template clone contains no Firebase, EDM4U, generated resolver repo, mobile service native/plugin root, service-specific Android template, IAP/cloud-service package, or service registry; retained runtime and Addressables behavior are unchanged. | fresh-clone, compile, editmode, boot, addressables-static, addressables-build, platform | implemented | Current worktree: Unity `6000.3.10f1` package resolve/compile exited 0; EditMode 12/12; PlayMode 1/1. Seven service roots and resolver settings are absent, custom Android-template flags are off, and manifest/lock contain no four removed service packages or two registry scopes. Addressables static/report checks returned 0 errors/warnings; report `buildlayout_2026.10.25.17.00.10.json` has 2 groups, 3 bundles, 0.02 MB and is byte-identical 2/2 vs C7. A normal clone of `20c4f30` resolved and compiled with exit 0, passed EditMode 12/12, kept the lockfile unchanged, and retained 0 service roots/markers after import. Follow-up `275afad` removed the final zero-reference IAP/AppLovin settings, added them to the invariant, and passed EditMode 12/12. Platform deferred because no target is selected. |
| template-identity | Player settings and platform baseline | A fresh template has neutral non-production identity, no saved platform profile or Jigsaw service config, and no product SDK/profile defines; applying the setup twice is stable. | compile, editmode, boot, editor-evidence, addressables-static, addressables-build, platform | implemented | Unity `6000.3.10f1` compile exited 0 with no `error CS`; EditMode passed 12/12; PlayMode passed 1/1. `TemplateProjectSettingsSetup.ApplyNeutralIdentity` exited 0 and a second application kept the `ProjectSettings.asset` SHA-256 unchanged. Legacy identity scan returned 0 matches. Addressables static/report checks returned 0 errors/warnings; `buildlayout_2026.10.25.16.47.08.json` has 2 groups, 3 bundles, 0.02 MB and is byte-identical 2/2 vs `13.39.02`. Platform build deferred because no target is selected. |
| package-exit-c6 | Thin-template handoff | The default template is bootable and package-free; docs/skills match the live tree, networking is demand-mounted through pinned Git UPM, and the two-tier Addressables baseline is reproducible. | compile, editmode, boot, addressables-static, addressables-build | implemented | Unity `6000.3.10f1` compile exited 0 with no `error CS`; EditMode passed 10/10; PlayMode passed 1/1. Static and report checks returned 0 errors/warnings. `buildlayout_2026.10.25.13.39.02.json`: 2 groups, 3 bundles, 0.02 MB; diff vs `13.03.10` was byte-identical for 2/2 groups. Platform/HAR deferred until a target and remote feature exist. |
| package-exit-c5 | Package and gitlink removal | A normal clone resolves packages, compiles, and runs template tests without private submodules or `com.paul.*` manifest entries. | fresh-clone, compile, editmode | implemented | Product commit `59a0465` removes 7 local dependencies, both gitlinks, and `.gitmodules`; Unity regenerated the lockfile with no internal package path. A fresh non-recursive clone of that commit had empty `git submodule status`; Unity `6000.3.10f1` exited 0 with no `error CS`, and EditMode passed 10/10. |
| package-exit-c4 | Legacy consumer retirement | Jigsaw roots, package-backed Addressables groups, and every serialized package GUID consumer are removed while the neutral bootstrap remains bootable. | compile, editmode, boot, addressables-static, addressables-build | implemented | Unity `6000.3.10f1` batch compile exited 0; EditMode passed 10/10; PlayMode passed 1/1. A scan of 1,423 package GUIDs found 0 references under `Assets/`. Addressables static and report checks exited 0 with 2 groups, 3 bundles, 0 errors, and 0 warnings. Two consecutive layout applications were byte-identical, and reports `buildlayout_2026.10.25.13.01.45.json` / `buildlayout_2026.10.25.13.03.10.json` had identical group sizes. |
| package-exit-c3 | Neutral bootstrap and UI shell | The loading scene uses a template-owned bootstrap, loads the global shell through an owned Addressables handle, and exposes local recovery plus init-tier Toast/MessageOverlay without a `com.paul.*` runtime dependency. | compile, editmode, boot, addressables-static | implemented | Unity `6000.3.10f1` batch compile exited 0. EditMode passed 10/10 (3 C3 boundary/prefab/group tests); PlayMode passed 1/1 (`LoadingScene_BootsNeutralShell`). `check.py --static` exited 0 with `game.local` 2 entries and `game.init` 3 entries; 495 legacy warnings remain for C4. |
| package-exit-c2 | Package dependency inventory | Every package assembly, C# consumer, serialized package GUID, and Addressables entry has an owner and disposition before deletion. | addressables-static, addressables-build | in_progress | Inventory: 18 assembly refs, 87 C# files, 1,104 serialized occurrences / 56 sources / 311 package GUIDs. `check.py --static` exited 0 with 495 warnings. A Unity 6000.3.10f1 Addressables build was attempted; no report was produced because `com.vfx.third_party` entry `Square.shadergraph` (GUID `3c0d5f6a750d0af418ec829d48071145`) cannot be imported. Report/diff deferred to removal of this known `PROJECT-ADDON` group in C4. |
| package-exit-c1 | Package-exit compile baseline | The legacy project compiles before package consumers are changed. | compile | implemented | Unity `6000.3.10f1` batch compile exited 0 on 2026-10-25 after correcting `PopupNoAdsData.cs`; log contained no `error CS`, script compilation took 2.46 s after a 376.27 s full import. |
| harness-cricket-tuning | Harness and Addressables audit tooling | The tuned Claude-first harness resolves its hooks and audit scripts, and the Unity project remains diagnosable after the package change. | compile, addressables-static | in_progress | `node --check` passed for both hooks; all 6 `tools/addressables/*.py` files parsed; `check.py --static` exited 0 with 0 errors and 485 warnings. Unity 6000.3.10f1 reached Roslyn but compile failed on the pre-existing Jigsaw file `PopupNoAdsData.cs`: `wnamespace` causes CS1513, CS1529, and CS1022. |

## Status Values

| Status | Meaning |
| --- | --- |
| planned | Accepted as intended behavior, not implemented |
| in_progress | Actively being built |
| implemented | Implemented and proof exists |
| changed | Contract changed after earlier implementation |
| retired | No longer part of the contract |

## Evidence Rules

- Every code change builds cleanly before it is called done.
- Unit proof covers logic that does not need a running system.
- Integration or end-to-end proof covers behavior that only manifests across a
  boundary or at runtime.
- When a change has no automated test, the minimum proof is observed evidence
  (a screenshot, a recorded request and response, a log excerpt), never an
  inference from a clean build.
- Behavior that must be checked on a platform, device, or environment the agent
  cannot reach is never "verified" locally — mark that proof deferred with a
  reason until someone checks it.
- Never mark a proof done because a tool was supposed to run it; record only
  what was actually run.
- A change may ship without every proof type if the story explains why.
