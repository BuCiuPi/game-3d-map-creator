# Validation

## Proof Strategy

Start from the green C7 commit, prove absence mechanically, then prove package
resolution and Unity behavior both in the current worktree and a normal fresh
clone. Addressables must remain byte-identical.

## Test Plan

| Proof | Cases |
| --- | --- |
| `compile` | retained assemblies compile after DLL/native/package removal |
| `editmode` | SDK roots, custom Android templates, packages, and registry scopes are absent |
| `boot` | loading scene reaches the neutral shell |
| `editor-evidence` | Unity import normalizes custom-template settings and regenerates the lock |
| `addressables-build` | fresh report is byte-identical to the C7 baseline |
| `platform` | deferred: no target platform is selected |
| `fresh-clone` | normal clone resolves packages, compiles, and runs EditMode without service artifacts |

## Fixtures

No account, SDK key, network, store, ad, or backend fixture is required.

## Acceptance Evidence

Current-worktree evidence on Unity `6000.3.10f1`:

- Package resolve and compile exited 0 with no C# or package-resolution error.
- EditMode passed 12/12 after the final absence/config invariant was added.
- PlayMode boot passed 1/1.
- Static Addressables validation returned 0 errors and 0 warnings.
- Content build produced `buildlayout_2026.10.25.17.00.10.json`; audit found
  2 groups, 3 bundles, and 0.02 MB. Both groups were byte-identical to the C7
  report `buildlayout_2026.10.25.16.47.08.json`.
- All seven service roots, the resolver files, four Unity service packages,
  two service registry scopes, and custom Android-template flags are absent.

Fresh-clone evidence for product commit `20c4f30`:

- A normal local clone completed with no submodule entry.
- Before and after Unity import, all service roots and package/registry markers
  remained absent.
- Unity package resolve and compile exited 0 with no C# or resolution error.
- EditMode passed 12/12, including the manifest, lockfile, registry, project
  settings, and asset-root absence invariant.
- Unity did not rewrite `Packages/packages-lock.json`.

Follow-up audit found two zero-reference service settings that were outside the
original roots: `Assets/Resources/BillingMode.json` from Unity IAP and
`ProjectSettings/AppLovinInternalSettings.json`. Commit `275afad` removes both,
adds them to the permanent file-absence invariant, and passed EditMode 12/12
with no compile or package-resolution error marker. The fresh-clone package
graph evidence above remains valid because this correction changes no package.

Device builds remain deferred because no target platform has been selected.
