# Inventory

Read on 2026-10-25 from baseline `231db8e`.

## Direct Package Baseline

`Packages/manifest.json` contains 31 non-module direct dependencies. Seven have
a current role and 24 have neither a live code consumer nor a retained
serialized GUID consumer.

| Keep | Evidence |
| --- | --- |
| `com.cysharp.r3` | `TemplateBootstrap` and `MessageOverlayView` use R3; `R3.Unity.asmdef` integrates it with Unity. |
| `com.github-glitchenzo.nugetforunity` | Owns the reproducible update path for the committed R3 core dependency set in `Assets/packages.config`. |
| `com.unity.addressables` | Runtime bootstrap and editor provisioning/build checks use it. |
| `com.unity.render-pipelines.universal` | The project render pipeline and 2D renderer assets require URP. |
| `com.unity.test-framework` | The template's EditMode and PlayMode proof suites require it. |
| `com.unity.ugui` | The three neutral shell views/prefabs use uGUI and TextMesh Pro. |
| `jp.co.cyberagent.smartaddresser` | The repeatable Addressables setup uses its editor capability. |

Codebase graph searches under `Assets/game.*` found zero candidate-package
consumers. A package-cache GUID map, including all `com.unity.2d.*` packages
owned by the 2D feature set, found zero references from retained `Assets/`,
`ProjectSettings/`, or `Packages/` files for every removal candidate.

## NuGet Payload

The tracked payload is 153 files / 12.74 MiB.

| Disposition | Package | Size | Reason |
| --- | --- | ---: | --- |
| remove | `ExtendedNumerics.BigDecimal` | 0.16 MiB | no code consumer |
| remove | `LanguageExt.Core` | 11.26 MiB | no code consumer |
| remove | `ObservableCollections` | 0.19 MiB | no code consumer or retained DLL dependency |
| remove | `ObservableCollections.R3` | 0.09 MiB | no code consumer |
| keep | `Microsoft.Bcl.AsyncInterfaces` | 0.12 MiB | precompiled reference of `R3.Unity.asmdef` |
| keep | `Microsoft.Bcl.TimeProvider` | 0.17 MiB | precompiled reference of `R3.Unity.asmdef` and dependency of `R3.dll` |
| keep | `R3` | 0.55 MiB | live template API |
| keep | `System.Threading.Channels` | 0.20 MiB | referenced by `R3.dll` |

After pruning, `R3` becomes the sole manually installed NuGet package;
AsyncInterfaces, TimeProvider, and Channels remain its dependency closure.

## Package-Owned Assets

The four Coffee packages own `Assets/ProjectSettings/` and three preloaded
assets in `ProjectSettings/ProjectSettings.asset`. No retained prefab, scene,
or game-owned asset references those package GUIDs.

Removing the Input System package also makes `activeInputHandler: 2` (`Both`)
stale. The retained uGUI shell uses the built-in legacy Input Manager, and Unity
adds URP's legacy debug axes during the first import. C8.2B therefore accepts
those generated axes and changes Active Input Handling to `Input Manager (Old)`.
