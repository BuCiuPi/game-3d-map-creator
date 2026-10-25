# 03 — Build And Profiles

Purpose: open, prove, and build the current internal-package-free,
provider-neutral template. Verified on 2026-10-25 during service-sdk-prune C8.1.

## Open The Project

1. Clone normally; do not use or initialize submodules.
2. Open the repository with Unity `6000.3.10f1`.
3. Open `Assets/game.contents/gameScenes/00_LoadingScene.unity` and press Play.

The loading scene is the only enabled scene in
`ProjectSettings/EditorBuildSettings.asset`.

## Proof Commands

Replace `<Unity>` and `<repo>` with absolute paths.

| Proof | Command |
| --- | --- |
| Compile | `"<Unity>/Editor/Unity.exe" -batchmode -quit -projectPath "<repo>" -logFile "<repo>/Logs/compile.log"` |
| EditMode | `"<Unity>/Editor/Unity.exe" -batchmode -projectPath "<repo>" -runTests -testPlatform EditMode -testResults "<repo>/Logs/editmode.xml" -logFile "<repo>/Logs/editmode.log"` |
| PlayMode boot | `"<Unity>/Editor/Unity.exe" -batchmode -projectPath "<repo>" -runTests -testPlatform PlayMode -testResults "<repo>/Logs/playmode.xml" -logFile "<repo>/Logs/playmode.log"` |
| Apply neutral identity | same prefix plus `-executeMethod Game.Bootstrap.Editor.Project.TemplateProjectSettingsSetup.ApplyNeutralIdentity -quit` |
| Apply layout | same prefix plus `-executeMethod Game.Bootstrap.Editor.Addressables.TemplateAddressablesSetup.ApplyTemplateLayout -quit` |
| Build content | same prefix plus `-executeMethod Game.Bootstrap.Editor.Addressables.TemplateAddressablesSetup.BuildContent -quit` |

On Windows, wait for the Unity process rather than only the launcher process.
Inspect the log and test XML; never infer success from launch alone.

## Addressables

- Profile: `Default`.
- Registered groups: `game.local` (2 entries) and `game.init` (3 entries).
- Both groups use local build/load paths and pack together.
- `game.local` is the default group.
- Remote catalog and Build With Player are off.
- SmartAddresser rule data:
  `Assets/AddressableAssetsData/AssetLayoutRules/template_LayoutRuleData.asset`.
- Setup/build entry:
  `Assets/game.bootstrap/Editor/Addressables/TemplateAddressablesSetup.cs`.
- Static/report tools: `tools/addressables/` via `bundle-optimization`.

There is no `02_partial` group until a real feature exists.
The active feature-group convention is `game.partial.<feature>`.

## Profiles And Player Builds

The neutral baseline is company `Amihan`, product `Unity Template`, application
identifier `com.amihan.unitytemplate`, version `0.1.0`, and build number `1`.
`TemplateProjectSettingsSetup` owns the repeatable Editor/batch setup. The values
are placeholders, not production identity.

The template has no saved platform build profile, release/CDN profile matrix,
player build argument contract, bundled service SDK, resolver cache, or custom
Android manifest/Gradle/proguard template. Product SDK/profile symbols are
absent. Removed vendored plugins leave no DOTween/Odin symbol. Package-managed
capabilities can still maintain symbols such as post processing; change those
only with the owning package during the separate dependency audit.
Do not call local paths remote or infer availability from a folder/group name;
validate resolved build/load paths.

## Optional Packages

The manifest contains no internal package, IAP package, Unity cloud-service
package, AppLovin registry, or Google EDM registry scope. For the first API or
service-backed feature, follow `docs/architecture/networking-on-demand.md`, add
the smallest pinned capability with its real consumer, commit manifest and
lockfile together, then run compile and tests. Provider-native files and build
configuration belong to that derived project, not this template.
