# Validation

## Required Proof

| Proof | Acceptance |
| --- | --- |
| consumer scan | zero retained C# and serialized GUID consumers |
| residue scan | none of the eight roots or removed define tokens remain |
| compile | Unity exits 0 with no C# or package-resolution error |
| editmode | all boundary and data tests pass |
| boot | neutral loading scene reaches `TemplateBootState.Ready` |
| addressables-static | 2 groups, 5 entries, 0 errors/warnings |
| addressables-build | report stays byte-identical to C8.1 for both groups |
| fresh-clone | normal clone compiles and passes EditMode without vendor roots |

## Acceptance Evidence

Implemented in product commit `7475d9b`.

- Pre-delete graph and serialized scans found zero retained consumers for 5,078
  subcohort GUIDs and all eight root-folder GUIDs.
- Removed 9,147 tracked files / 581.03 MiB. Repository residue scan found zero
  candidate roots and zero DOTween/Odin PlayerSettings symbols.
- Unity project-settings setup and compile exited 0 with no C# or package
  resolution error. The retained Rider package still logged its known missing
  local `EditorPlugin` path; it is package-audit scope, not a compile failure.
- EditMode passed 12/12 and PlayMode boot passed 1/1.
- Addressables static and report checks returned 0 errors and 0 warnings.
  `buildlayout_2026.10.25.17.43.30.json` contains 2 groups, 3 bundles, and
  0.02 MB; both groups are byte-identical to C8.1 report `17.00.10`.
- A normal clone of `7475d9b` contained no candidate roots or submodules.
  Fresh package resolve/import/compile exited 0 in 261.3 seconds with no error
  marker, compared with the earlier 376-second package-exit full-import
  baseline. EditMode passed 12/12. After import, vendor roots and defines stayed
  absent and `Packages/packages-lock.json` stayed unchanged.

Unity import still rewrote retained Coffee, Code Coverage, PlayerSettings, and
ShaderGraph settings in the disposable clone. Those package-owned side effects
are deferred to the UPM/package audit.
