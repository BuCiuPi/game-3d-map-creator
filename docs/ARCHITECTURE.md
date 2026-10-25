# Architecture

This is the current one-page module map.
Detail lives in `.claude/knowledge/`; decisions live in `docs/decisions/`.

## System Overview

unity-template is a Unity `6000.3.10f1` starting project with a neutral boot,
minimal shell UI, DT/DA primitives, and a two-tier local Addressables baseline.
It contains no game feature, backend, auth, internal package, or submodule by
default.

## Module Map

| Module | Path | Responsibility |
| --- | --- | --- |
| `game.core.runtime` | `Assets/game.core/Runtime/` | DT/DA primitives |
| `game.ui.runtime` | `Assets/game.ui/Runtime/` | loading, recovery, Toast, MessageOverlay, global shell views |
| `game.bootstrap.runtime` | `Assets/game.bootstrap/Runtime/` | neutral boot and shell handle ownership |
| `game.bootstrap.addressables.editor` | `Assets/game.bootstrap/Editor/Addressables/` | idempotent layout and content-build entry |
| tests | `Assets/game.core/Tests/`, `Assets/game.bootstrap/Tests/` | data, boundary, prefab/group and boot proof |
| Addressable content | `Assets/game.assetbundles/` | explicit game-owned shipping assets |
| scene | `Assets/game.contents/gameScenes/00_LoadingScene.unity` | local boot scene |
| package support | `Assets/TextMesh Pro/`, `Assets/Packages/` | TMP resources and the four-DLL R3 dependency closure; not template modules |

## Dependency Direction

`game.bootstrap.runtime` → `game.ui.runtime` and Unity Addressables.
`game.core.runtime` and `game.ui.runtime` do not depend on bootstrap or an
internal package.
Boundary tests reject the retired `com.paul.*`/`Paul.*` dependency surface.

## Entry Point

`TemplateBootstrap` in the loading scene instantiates the init-tier global shell,
owns the operation handle, publishes ready/failure state, and releases on
destroy.
The local loading and recovery surfaces do not require catalog or global DI.

## Data And State

The template contains authoring primitives, not project data or player state.
`DataAsset<TData>` holds one typed payload; `DataTable<TRow>` provides a typed
list/index contract.
See `docs/architecture/template-core-data.md`.

## Content Delivery

SmartAddresser owns membership for `game.local` and `game.init`.
All five entries are below `Assets/game.assetbundles/`.
`TemplateAddressablesSetup` applies the folder rules and builds content;
`tools/addressables/` checks static configuration and build reports.

## Optional Capabilities

A derived project installs an internal capability only at its first consumer,
through Git UPM pinned to a commit SHA.
Networking setup is `docs/architecture/networking-on-demand.md`.
Do not copy package code into `Assets/` or recreate repository submodules.
The live direct non-module baseline is R3, NuGetForUnity, Addressables, URP,
Test Framework, uGUI, and SmartAddresser (decision `0012`).

## Frozen And High-Risk Areas

- The public `game.core` API.
- The loading scene, bootstrap lifecycle and UI-shell serialized wiring.
- Addressables group/profile/load paths, layout rules, content state and GUIDs.
- `Packages/manifest.json`, `Packages/packages-lock.json`, player settings and
  build scenes.
- Any optional third-party capability added under `Assets/` or `Packages/`.

## Default Principles

- Inner feature logic does not depend on UI or transport details.
- Parse unknown data once at the boundary.
- One owner writes each shared state and owns each Addressables handle.
- Contract producers and consumers change together.
- Record choices that constrain later work in `docs/decisions/`.
