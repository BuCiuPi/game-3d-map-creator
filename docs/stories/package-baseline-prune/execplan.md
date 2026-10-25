# Exec Plan

## Goal

Reduce the package baseline to the seven current capabilities and the four-DLL
R3 dependency closure without changing neutral boot, DT/DA, UI shell, URP, or
Addressables behavior.

## Risk Classification

High-risk: package resolution, transitive lock regeneration, preloaded assets,
precompiled assemblies, editor import behavior, and fresh-clone reproducibility.

## Exact Proposed Change

```diff
 Packages/manifest.json
- com.benoitfreslon.vibration
- com.coffee.softmask-for-ugui
- com.coffee.ui-effect
- com.coffee.ui-particle
- com.coffee.upm-git-extension
- com.cysharp.unitask
- com.unity.cinemachine
- com.unity.collab-proxy
- com.unity.device-simulator.devices
- com.unity.feature.2d
- com.unity.feature.development
- com.unity.ide.rider
- com.unity.ide.visualstudio
- com.unity.inputsystem
- com.unity.mathematics
- com.unity.mobile.android-logcat
- com.unity.multiplayer.center
- com.unity.nuget.newtonsoft-json
- com.unity.postprocessing
- com.unity.recorder
- com.unity.timeline
- com.unity.visualscripting
- jp.hadashikick.vcontainer
- pl.bravasoft.unity.polly
- scoped registry package.openupm.com / com.coffee

- Assets/ProjectSettings{,.meta}
- PlayerSettings preloaded GUID 0093cf062d3a40e4f92d7b552abf271f
- PlayerSettings preloaded GUID 6c7031edf1c26244e8f6e935d0327318
- PlayerSettings preloaded GUID 754c89d835987084a9c79a957ecf6540
~ PlayerSettings activeInputHandler: Both -> Input Manager (Old)
~ ProjectSettings/InputManager.asset: retain Unity-generated URP legacy debug axes

- Assets/Packages/ExtendedNumerics.BigDecimal.3003.0.0.346{,.meta}
- Assets/Packages/LanguageExt.Core.4.4.9{,.meta}
- Assets/Packages/ObservableCollections.3.3.4{,.meta}
- Assets/Packages/ObservableCollections.R3.3.3.4{,.meta}
~ Assets/packages.config: retain R3 plus AsyncInterfaces, TimeProvider, Channels
~ Packages/packages-lock.json: regenerate and review direct/transitive graph
+ TemplateProjectSettingsSetup enforces the legacy input baseline
+ TemplateBoundaryTests direct-package, input, and removed-root invariants
+ docs/decisions/0012-minimal-live-package-baseline.md
```

Retain all explicit `com.unity.modules.*` entries in this phase. Retain R3,
NuGetForUnity, Addressables, URP, Test Framework, uGUI, and SmartAddresser.

## Work Phases

1. Freeze baseline `231db8e`, code consumers, package GUID map, and DLL metadata.
2. Record decision `0012` and this exact diff.
3. Apply the manifest, Coffee settings, preloaded asset, and NuGet payload edits.
4. Let Unity resolve the package graph, then review every lockfile delta.
5. Run residue, compile, EditMode, boot, Addressables static/build/report diff,
   and normal fresh-clone proofs.
6. Record evidence and defer built-in module pruning to a separate cohort.

## Stop Conditions

Stop and restore the affected dependency if package resolution fails, retained
code or serialized assets require it, the R3 assembly closure breaks, a
game-owned GUID changes, Addressables layout changes, or a normal clone cannot
compile from tracked files.
