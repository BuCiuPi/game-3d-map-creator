# Exec Plan

## Goal

Make a fresh clone start from a neutral, reproducible identity without choosing
a product platform or retaining Jigsaw app credentials.

## Scope

In scope:

- Add an idempotent Unity Editor setup command and baseline invariant test.
- Apply the command so Unity serializes player settings.
- Remove unreferenced Android build profiles and Jigsaw Firebase app configs.
- Synchronize the generated Android resolver cache bundle ID.
- Record SDK/define disposition for the next dependency-prune task.

Out of scope:

- Removing SDK implementations or package dependencies.
- Creating production bundle IDs, signing, remote delivery, or platform builds.

## Risk Classification

Risk flags (from `docs/FEATURE_INTAKE.md`):

- Serialized assets.
- Build profiles.
- Template boundary.
- Auth / SDKs.

Hard gates:

- None; no vendored plugin implementation or public `game.core` API changes.

## Proposed Change

```csharp
PlayerSettings.companyName = "Amihan";
PlayerSettings.productName = "Unity Template";
PlayerSettings.bundleVersion = "0.1.0";
PlayerSettings.Android.bundleVersionCode = 1;
PlayerSettings.iOS.buildNumber = "1";

PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, Identifier);
PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, Identifier);
PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, Identifier);

foreach (var target in GetAvailableBuildTargets())
{
    var current = PlayerSettings.GetScriptingDefineSymbols(target)
        .Split(';', StringSplitOptions.RemoveEmptyEntries);
    var neutral = current.Where(symbol => !ProductDefines.Contains(symbol)).ToArray();
    PlayerSettings.SetScriptingDefineSymbols(target, neutral);
}
```

```diff
+ Assets/game.bootstrap/Editor/Project/TemplateProjectSettingsSetup.cs
+ Assets/game.bootstrap/Editor/Project/game.bootstrap.project.editor.asmdef
+ Template identity invariant in TemplateBoundaryTests
- Assets/Settings/Build Profiles/
- Assets/google-services.json.backup
- Assets/StreamingAssets/google-services-desktop.json
~ ProjectSettings/ProjectSettings.asset (written by Unity)
~ ProjectSettings/AndroidResolverDependencies.xml (cache bundle ID only)
```

## Work Phases

1. Inventory legacy identity, build profiles, defines, SDK consumers, and GUID references.
2. Lock the neutral baseline and non-goals in decision `0009`.
3. Add setup code and EditMode acceptance coverage.
4. Apply through Unity batch mode and remove obsolete serialized/config assets.
5. Run compile, EditMode, PlayMode, static scan, and Addressables regression proof.
6. Update knowledge, matrix, validation, and trace records.

## Stop Conditions

Pause for human confirmation if:

- Applying the baseline would select a platform or alter boot/Addressables behavior.
- A removed config has a live consumer in `Assets/game.*`.
- Clearing a define creates a compile dependency on a retained SDK.
- Unity changes an unrelated asset GUID or address.
