# Inventory

Read on 2026-10-25 from baseline `bae1af6`.

## Cohort Size

| Root | Tracked files | Size |
| --- | ---: | ---: |
| `Assets/UnityAssetStore/` | 4,781 | 520.14 MiB |
| `Assets/AlmostEngine/` | 1,734 | 37.12 MiB |
| `Assets/Plugins/` | 1,452 | 18.25 MiB |
| `Assets/StompyRobot/` | 622 | 3.38 MiB |
| `Assets/Spine/` | 543 | 2.14 MiB |
| `Assets/UnitySkipSplashScreen/` | 7 | less than 0.01 MiB |
| `Assets/Editor/` | 3 | less than 0.01 MiB |
| `Assets/Resources/` | 5 | less than 0.01 MiB |
| **Total** | **9,147** | **581.03 MiB** |

`Assets/Editor/` only owns `SpineSettings`. `Assets/Resources/` only owns
`DOTweenSettings` and an orphaned empty `I2Languages` asset after C8.1.

## Consumer Evidence

- Codebase graph search under `Assets/game.*` found no DOTween, Odin, Spine,
  Feel, MoreMountains, Lean/CW, AlmostEngine, SRDebugger, Best HTTP, screenshot,
  selection, scroller, or PSD-importer consumer. The only SRDebugger hits are
  forbidden-define guards.
- 5,078 GUIDs owned by the 15 subcohorts had zero references from game-owned
  assets, Addressables settings, project settings, or retained build assets.
- A repository-wide serialized scan found only two cross-root references:
  `Assets/Resources/DOTweenSettings.asset` points into DOTween and
  `Assets/Editor/SpineSettings.asset` points into Spine. Both settings are in
  this removal cohort.
- The eight root-folder GUIDs have zero external references.
- No current Addressables entry is owned by any candidate root.

## Retained Boundary

The phase retains `Assets/TextMesh Pro/`, `Assets/Packages/`,
`Assets/ProjectSettings/`, all `Assets/game.*` roots, URP settings, and the live
package manifest. Their package/value audit follows independently so a vendor
asset deletion cannot hide package-lock churn.
