# 02 — Runtime And Wiring

Purpose: describe the neutral runtime and its assembly boundaries.
Verified on 2026-10-25 after package-baseline-prune C8.2B (`9423722`).

## Assemblies

| Assembly | Root | Direct project/runtime references |
| --- | --- | --- |
| `game.core.runtime` | `Assets/game.core/Runtime/` | none |
| `game.ui.runtime` | `Assets/game.ui/Runtime/` | TMP, uGUI, R3 Unity |
| `game.bootstrap.runtime` | `Assets/game.bootstrap/Runtime/` | `game.ui.runtime`, Addressables, ResourceManager, R3 Unity |
| `game.bootstrap.addressables.editor` | `Assets/game.bootstrap/Editor/Addressables/` | SmartAddresser Editor, Addressables Editor |
| `game.core.tests.editmode` | `Assets/game.core/Tests/EditMode/` | `game.core.runtime` |
| `game.bootstrap.tests.editmode` | `Assets/game.bootstrap/Tests/EditMode/` | bootstrap, UI, Addressables Editor |
| `game.bootstrap.tests.playmode` | `Assets/game.bootstrap/Tests/PlayMode/` | bootstrap, UI |

The runtime/editor boundary test rejects `com.paul.*` assembly references and
`Paul.*` namespaces.

## Boot Sequence

Entry scene: `Assets/game.contents/gameScenes/00_LoadingScene.unity`.

1. The scene-wired loading and recovery views exist without Addressables
   instantiation or global DI.
2. `TemplateBootstrap.Start` sets the state to booting and instantiates its
   serialized global-shell `AssetReference` below the shell root.
3. On success it retains the operation handle, hides loading/recovery, and sets
   `TemplateBootState.Ready`.
4. On failure it releases a valid handle, publishes failure, and shows recovery.
5. `OnDestroy` releases the retained instantiated shell handle.

The PlayMode proof is
`TemplateBootPlayModeTests.LoadingScene_BootsNeutralShell`.

## UI Shell

| Availability | Prefab | View |
| --- | --- | --- |
| local | `P_BootLoadingOverlay` | `LoadingOverlayView` |
| local | `P_BootRecoveryOverlay` | `MessageOverlayView` |
| init | `P_GlobalUiShell` | `GlobalUiShellView` |
| init | `P_Toast` | `ToastView` |
| init | `P_MessageOverlay` | `MessageOverlayView` |

Feature screens, feature loading, shop, quest, liveops, gameplay, and project
navigation do not belong to this shell.

## Data

`Assets/game.core/Runtime/Data/` contains only `DataAsset<TData>`,
`DataTable<TRow>`, `DataTableBase`, and `DataTableRow`.
See `docs/architecture/template-core-data.md` for contracts and placement.

## Optional Composition

The template does not create an application `LifetimeScope`, service facade,
gateway, event hub, persistence layer, or API client.
A DI container and UniTask are not installed in the baseline.
A derived feature owns any scope or service it introduces.
Networking setup starts at `docs/architecture/networking-on-demand.md`.
