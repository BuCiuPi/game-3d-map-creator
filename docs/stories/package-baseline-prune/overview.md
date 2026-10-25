# Overview

## Current Behavior

The template declares 31 non-module dependencies in `Packages/manifest.json`,
although the live shell only consumes R3, Addressables, URP, uGUI/TextMesh Pro,
the Unity Test Framework, and SmartAddresser. The remaining direct packages add
editor startup work, transitive packages, settings, and upgrade surface to every
derived game without providing a template capability.

The committed NuGet payload also contains BigDecimal, LanguageExt, and
ObservableCollections assemblies that no retained source or serialized asset
uses.

## Target Behavior

The manifest declares only dependencies with a current template consumer or a
required dependency-management role. Optional gameplay, editor, IDE, service,
and visual packages are added by a derived game with their first real consumer.

## Scope

- Remove 24 zero-consumer direct UPM/Git dependencies.
- Remove the Coffee package registry, package-owned settings assets, and their
  three PlayerSettings preloads.
- Remove four zero-consumer NuGet payloads while retaining the four assemblies
  required by the live R3 integration.
- Regenerate and review `Packages/packages-lock.json` independently from the
  earlier asset deletion cohort.
- Add mechanical guards for the direct package baseline and removed roots.

## Non-Goals

- Removing or auditing explicit `com.unity.modules.*` entries; that is a later,
  independent cohort.
- Replacing R3, Addressables, URP, uGUI/TextMesh Pro, Test Framework, or
  SmartAddresser.
- Migrating the committed R3 NuGet binaries to a different distribution model.
- Adding HyperEel or game-specific feature packages before they have a consumer.
