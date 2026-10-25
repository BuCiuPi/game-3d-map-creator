# Validation

## Proof Strategy

Prove the serialized identity through an EditMode invariant, then prove the
template still compiles, boots, and produces the same Addressables content.
Static scans must find no Jigsaw identity outside historical docs.

## Test Plan

| Proof | Cases |
| --- | --- |
| `compile` | Editor setup API and cleared-define compilation |
| `editmode` | company, product, version, app IDs, version codes, and absence of product SDK/profile defines |
| `boot` | loading scene reaches the neutral shell |
| `editor-evidence` | setup batch log reports the applied baseline |
| `addressables-build` | static check plus build/report diff against the C6 baseline |
| `platform` | deferred: no target platform has been selected |

## Fixtures

No account, network, Firebase, ads, or backend fixture is required.

## Acceptance Evidence

- Unity `6000.3.10f1` batch compile exited `0`; `c7-compile.log` contains no
  `error CS`.
- EditMode passed `12/12`; PlayMode boot passed `1/1`.
- `ApplyNeutralIdentity` exited `0`; a second run kept
  `ProjectSettings.asset` SHA-256
  `5B7D76374506EEC958D7601F7E9646F9EDF062F207BAE5FC20017423683C3273`.
- The legacy identity scan found no Jigsaw company, product, or application ID
  under `Assets/`, `Packages/`, or `ProjectSettings/`.
- Addressables static and fresh-report checks returned 0 errors/warnings.
  Report `buildlayout_2026.10.25.16.47.08.json` contains 2 groups, 3 bundles,
  and 0.02 MB; both groups are byte-identical to `13.39.02`.
- Review verdict: PASS, no confirmed or plausible findings in the C7 delta.
- Deferred: platform build and signing because the target platform remains open.
