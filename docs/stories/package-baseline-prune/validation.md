# Validation

## Required Proof

| Proof | Acceptance |
| --- | --- |
| consumer scan | zero live code and serialized GUID consumers for all removals |
| package graph | only seven non-module direct dependencies; lock has no orphan direct entries |
| NuGet closure | R3 plus exactly AsyncInterfaces, TimeProvider, and Channels compile |
| residue scan | removed package IDs, Coffee settings/preloads, and four NuGet roots are absent |
| compile | Unity exits 0 with no C# or package-resolution error |
| editmode | all boundary and data tests pass |
| boot | neutral loading scene reaches `TemplateBootState.Ready` |
| addressables | static checks pass and both groups remain byte-identical to C8.2A |
| fresh-clone | normal clone resolves, compiles, and passes EditMode from tracked files |

## Acceptance Evidence

Implemented in product commit `9423722`.

- Codebase graph searches and a package-cache GUID map found zero retained code
  or serialized consumers for every removed direct package, including all
  `com.unity.2d.*` packages in the removed feature set.
- The manifest now has exactly seven non-module direct dependencies. Unity
  regenerated the lock from 94 to 55 packages: 39 removed, 0 added, and only
  four retained packages changed depth.
- Removed 50 tracked files / 11.70 MiB from the four unused NuGet roots and nine
  Coffee settings/root files. `Assets/packages.config` now contains R3 plus
  AsyncInterfaces, TimeProvider, and Channels; R3 is the sole manual package.
- Active Input Handling is now `Input Manager (Old)`. Unity's generated URP
  legacy debug axes are committed, and two setup applications kept
  `ProjectSettings.asset` and `InputManager.asset` SHA-256 stable.
- Current worktree setup/compile exited 0 with no C# or package error marker.
  EditMode passed 12/12 and PlayMode boot passed 1/1.
- Addressables static and report checks returned 0 errors and 0 warnings.
  `buildlayout_2026.10.25.18.14.09.json` contains 2 groups, 3 bundles, and
  0.02 MB; both groups are byte-identical to C8.2A report `17.43.30`.
- A normal clone of `9423722` had no submodules. Full package resolve/import and
  compile completed in 137.8 seconds with 0 error markers, compared with 261.3
  seconds before C8.2B. The lock stayed byte-stable, removed roots stayed absent,
  EditMode passed 12/12, and `git diff --quiet` returned 0 after import/tests.

Platform build remains deferred because the template has no selected target.
