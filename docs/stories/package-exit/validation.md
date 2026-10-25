# Validation

## Proof Strategy

Each changeset must pass its gate in
`artifacts/unity-template-package-exit-plan.html`. Package removal is complete
only after a fresh clone resolves and compiles without submodules.

## Test Plan

| Proof | Cases |
| --- | --- |
| `compile` | Every C# or `.asmdef` change; final fresh clone |
| `editmode` | `game.core.tests.editmode` after structural changes |
| `boot` | Loading scene reaches the neutral shell |
| `editor-evidence` | Shell prefab wiring and every retained moved asset |
| `addressables-static` | No error and no entry under removed package paths |
| `addressables-build` | Build report/check/audit/diff after package asset removal |
| `platform` | Deferred until the owner chooses the main platform |

## Fixtures

- Unity `6000.3.10f1`.
- Existing submodules at their pinned commits until C5.
- A separate fresh clone for the C5 acceptance gate.

## Acceptance Evidence

- C1, 2026-10-25: Unity batch compile exited 0 with no `error CS`; script
  compilation 2.46 s after a 376.27 s full import.
- C2, 2026-10-25: dependency inventory mapped 18 assembly references, 87
  C# files, and 1,104 serialized occurrences from 56 source files to 311
  package GUIDs. `check.py --static` exited 0 with 495 warnings. A fresh
  Addressables build was attempted but produced no report because the
  `com.vfx.third_party` `Square.shadergraph` entry cannot be imported; the
  known `PROJECT-ADDON` group is removed in C4 before the first report/diff.
- C3, 2026-10-25: Unity batch compile exited 0. EditMode passed 10/10,
  including the three new assembly-boundary, prefab-wiring, and tier/group
  ownership tests. PlayMode passed 1/1: `LoadingScene_BootsNeutralShell` reached
  `TemplateBootState.Ready` and found `GlobalUiShellView`. Static Addressables
  check exited 0 with 2 `game.local` entries, 3 `game.init` entries, and 495
  warnings from legacy/package content assigned to C4.
- C4, 2026-10-25: 1,423 package GUIDs were scanned with 0 references remaining
  under `Assets/`. Static and report Addressables checks passed with 2 groups,
  3 bundles, 0 errors, and 0 warnings. Consecutive applications of the template
  layout were byte-identical; consecutive reports
  `buildlayout_2026.10.25.13.01.45.json` and
  `buildlayout_2026.10.25.13.03.10.json` had identical group sizes. Unity batch
  compile exited 0, EditMode passed 10/10, and PlayMode passed 1/1.
- C5, 2026-10-25: product commit `59a0465` removes both gitlinks,
  `.gitmodules`, and seven local package dependencies. Unity regenerated the
  lockfile with no `com.paul.*`, `game-packages`, or `vfx-packages` reference.
  A fresh clone made without `--recurse-submodules` had empty
  `git submodule status`; Unity `6000.3.10f1` exited 0 with no C# compilation
  error and EditMode passed 10/10 after a full import.
- C6, 2026-10-25: Unity batch compile exited 0 with no `error CS`; EditMode
  passed 10/10 and PlayMode boot passed 1/1. Addressables static and fresh-report
  checks exited 0 with no errors or warnings. Report
  `buildlayout_2026.10.25.13.39.02.json` contains 2 groups, 3 bundles, and
  0.02 MB; its two groups are byte-identical to
  `buildlayout_2026.10.25.13.03.10.json`. Project/harness docs now describe the
  package-free tree and the Git UPM networking-on-demand gate. Platform build
  and HAR remain explicitly deferred until a target and remote feature exist.
