# Exec Plan

## Goal

Produce a bootable Unity template whose fresh clone has no internal submodule or
package dependency.

## Scope

In scope:

- Execute changesets C1-C6 in
  `artifacts/unity-template-package-exit-plan.html`.
- Replace only neutral boot and UI capabilities.
- Remove legacy consumers, package assets, manifest entries, and gitlinks.

Out of scope:

- Add a backend/API feature or HyperEel dependency.
- Port Jigsaw business modules.
- Change the `game.core` data API.

## Risk Classification

Risk flags (from `docs/FEATURE_INTAKE.md`):

- Package/submodule boundary, `.asmdef`, bootstrap, serialized GUIDs,
  Addressables groups and paths.

Hard gates:

- Compile baseline before dependency changes.
- Zero unknown consumers before package deletion.
- Preserve `.meta` GUIDs for retained assets.
- Fresh-clone compile before completion.

## Work Phases

1. C1: green compile baseline.
2. C2: complete dependency/GUID/Addressables disposition map.
3. C3: neutral bootstrap and UI shell.
4. C4: retire Jigsaw and VFX consumers.
5. C5: remove package assemblies, manifest entries, and gitlinks.
6. C6: full proof, docs, and Addressables handoff.

## C3 Reviewed Delta

The neutral shell adds these concrete files before any legacy deletion:

```text
Assets/game.bootstrap/Runtime/
  game.bootstrap.runtime.asmdef
  TemplateBootstrap.cs
  TemplateBootState.cs
Assets/game.bootstrap/Tests/{EditMode,PlayMode}/
Assets/game.ui/Runtime/
  game.ui.runtime.asmdef
  LoadingOverlayView.cs
  MessageOverlayView.cs
  ToastView.cs
  GlobalUiShellView.cs
Assets/game.assetbundles/00_local/ui/shell/
  P_BootLoadingOverlay.prefab
  P_BootRecoveryOverlay.prefab
Assets/game.assetbundles/01_init/ui/00_shared/
  P_Toast.prefab
  P_MessageOverlay.prefab
  P_GlobalUiShell.prefab
```

`TemplateBootstrap` owns one soft `AssetReference` to the global shell and the
handle returned by instantiation. Its boot path is intentionally limited to:

```csharp
loadingOverlay.Show("Preparing template shell...");
var handle = globalUiShell.InstantiateAsync(shellRoot);
// success: hide loading and publish Ready
// failure: show the scene-wired recovery overlay
```

SmartAddresser owns the two new groups: `game.local` for `00_local` and
`game.init` for `01_init`. Both resolve through the current local profile; no
group YAML membership is edited by hand. The existing Jigsaw runtime
initializers are disabled in C3 so only `TemplateBootstrap` owns startup; their
files are deleted with the rest of the legacy consumers in C4.

## C4 Reviewed Delta

C4 removes complete Jigsaw-owned roots rather than extracting their framework
or business layers:

```text
DELETE Assets/game.art/
DELETE Assets/game.contents.gameplay/
DELETE Assets/game.shared/
DELETE Assets/game.visualization/
DELETE Assets/game.contents/{Data,Editor,effects,LifeTimeScope,sounds,ui,UI_Toolkit}/
DELETE Assets/game.contents/gameScenes/01_LobbyScene.unity
KEEP   Assets/game.contents/gameScenes/00_LoadingScene.unity
```

The current SmartAddresser setup is package-backed and is deleted with
`game.shared`. Before that deletion, C4 adds a template-owned editor assembly at
`Assets/game.bootstrap/Editor/Addressables/`. Its public operation is:

```csharp
TemplateAddressablesSetup.ApplyTemplateLayout();
```

That operation ensures `game.local` and `game.init`, assigns local build/load
paths, keeps `game.local` as the default group, synchronizes the existing
`template_LayoutRuleData` without replacing stable rule IDs, and applies the two
tier-folder rules through SmartAddresser. A one-run migration command removes
the following legacy groups with Addressables Editor APIs, then deletes itself:

```text
ui_local
game.visualization.common
scenes
com.paul.game.visualization.common
game.visualization.master_data
Default Local Group
com.vfx.third_party
com.vfx.shared.assets
```

Package-owned group assets are detached from project settings, not deleted from
the pinned submodules. Project-owned groups, orphan group assets, schemas, old
layout rules, and the roots listed above are removed through `AssetDatabase`.
Build Settings is reduced to the preserved loading scene. After the one-run
migration, `ApplyTemplateLayout` is run twice with a clean-tree comparison; the
second application must produce no semantic diff.

## C5 Reviewed Outcome

C5 removes package infrastructure only after the C4 consumer scan reached
zero. The exact product delta is:

```diff
- Packages/game-packages  (gitlink)
- Packages/vfx-packages   (gitlink)
- .gitmodules
- 7 com.paul.* file dependencies in Packages/manifest.json
- their regenerated Packages/packages-lock.json entries
```

No replacement package is installed by default. A project adds a capability
only when its first real consumer appears, through a Git UPM dependency pinned
to a commit SHA as accepted in decision `0008`.

Acceptance ran against a normal fresh clone of product commit `59a0465`, not
the workspace Library cache. The clone had no submodule metadata or directories;
Unity `6000.3.10f1` resolved, compiled, exited 0, and passed EditMode 10/10.

## C6 Reviewed Outcome

C6 rebaselines the operating docs and skills against the package-free tree.
The API/gateway skills are dormant until a derived project mounts a direct,
commit-pinned Git UPM capability and records its actual contract in
`.claude/api-codegen-config.md`. The exact networking recipe is
`docs/architecture/networking-on-demand.md`; no networking package was added to
the template.

Final current-worktree proof on 2026-10-25:

```text
compile                 Unity exit 0; 0 error CS matches
EditMode                10 / 10 passed
PlayMode boot           1 / 1 passed
Addressables static     0 errors, 0 warnings
Addressables report     buildlayout_2026.10.25.13.39.02.json
report audit            2 groups, 3 bundles, 0.02 MB
report diff             2 / 2 groups byte-identical vs 13.03.10
```

The Addressables modernization artifact now treats P1 and the current local P2
baseline as complete. P3-P7 remain demand-driven: platform/profile work waits
for a chosen target, and `02_partial` waits for a real feature consumer.

## Stop Conditions

Pause for human confirmation if:

- A retained asset cannot preserve its GUID or address.
- An unknown consumer remains before package deletion.
- The `game.core` public API would change.
- Neutral boot requires auth/backend behavior.
- A validation requirement would be weakened.
