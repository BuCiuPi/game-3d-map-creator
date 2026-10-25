# Exec Plan

## Goal

Remove the complete zero-consumer vendored asset cohort without changing the
neutral boot, UI shell, DT/DA API, Addressables layout, or package graph.

## Risk Classification

High-risk: serialized assets, `.asmdef` files, root `.meta` GUIDs, large binary
deletion, scripting symbols, and the inherited template boundary.

## Exact Proposed Change

```diff
- Assets/UnityAssetStore{,.meta}
- Assets/AlmostEngine{,.meta}
- Assets/StompyRobot{,.meta}
- Assets/Plugins{,.meta}
- Assets/Spine{,.meta}
- Assets/UnitySkipSplashScreen{,.meta}
- Assets/Editor{,.meta}
- Assets/Resources{,.meta}
- PlayerSettings symbols: DOTWEEN, ODIN_VALIDATOR*, ODIN_INSPECTOR*
+ TemplateProjectSettingsSetup forbidden-define entries for the removed plugins
+ TemplateBoundaryTests absence checks for all eight roots and removed defines
~ CLAUDE.md, knowledge/architecture docs, test matrix, trace, and this plan artifact
```

No `Packages/manifest.json`, `Packages/packages-lock.json`, Addressables group,
address, label, or game-owned GUID changes in this phase.

## Work Phases

1. Freeze baseline `bae1af6`, graph search, cohort sizes, and GUID-reference map.
2. Record decision `0011` and this exact removal list.
3. Delete each complete root with its root `.meta` file.
4. Clear DOTween/Odin symbols through the repeatable project-settings setup.
5. Run repository residue/GUID scans, compile, EditMode, PlayMode boot, and
   Addressables static/build/report diff.
6. Commit the product delta and prove a normal fresh clone.
7. Record evidence and begin the separate UPM/package value audit.

## Stop Conditions

Stop and restore the affected cohort if a retained asset references one of its
GUIDs, a retained assembly requires it, a game-owned GUID changes, Addressables
layout changes, or a normal clone cannot compile without it.
