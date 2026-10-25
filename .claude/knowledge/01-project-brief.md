# 01 — Project Brief

Purpose: define what this shared template is and what it deliberately omits.
Verified on 2026-10-25 during package-baseline-prune C8.2B.

## What It Is

The team's shared Unity project from which new game repositories start.
A change here affects future projects, not projects already created from it.

- Unity `6000.3.10f1`, URP `17.3.0` with the 2D renderer.
- R3, Addressables `2.9.0`, SmartAddresser, uGUI/TMP, and Test Framework.
- No internal package, backend, auth, analytics, ads, game module, or submodule by
  default.

## Current Boundary

| Root | Ownership |
| --- | --- |
| `Assets/game.core/` | DT/DA primitives copied into every derived project |
| `Assets/game.bootstrap/` | neutral boot plus Addressables setup/build tooling |
| `Assets/game.ui/` | minimal reusable shell UI |
| `Assets/game.assetbundles/00_local/` | loading and recovery assets available at boot |
| `Assets/game.assetbundles/01_init/` | global shell, Toast, MessageOverlay after init |
| `Assets/game.contents/gameScenes/00_LoadingScene.unity` | the only project scene |

A feature creates `Assets/game.assetbundles/02_partial/<feature>/` and its own
runtime assembly when the project needs it.

## Heritage

The repository was cut from Jigsaw.
Its game/shared/visualization code, assets, player identity, build profiles, and
service integrations are gone. It contains no bundled Firebase, EDM4U,
generated Maven repository, mobile service native library, IAP package, Unity
cloud-service package, service-specific Android build template, vendored art
pack, sample, runtime plugin, or editor tool. Its non-module manifest baseline is
seven live capabilities; its NuGet payload is only the four-DLL R3 closure.

## Decisions

- Thin, bootable core; no placeholder framework/business hierarchy.
- All explicit game-owned Addressable assets live below
  `Assets/game.assetbundles/`.
- SmartAddresser is the membership owner; setup is idempotent.
- Learn capabilities from HyperEel common core and Cricket research without
  copying their package modules, source, namespaces, addresses, or game rules.
- A derived project installs optional capability through Git UPM at an accepted
  commit only when its first real consumer appears (decision `0008`).
- A service SDK and its platform configuration arrive with that first consumer;
  the shared template carries no provider sample (decision `0010`).
- Vendored assets and tools follow the same first-consumer rule (decision `0011`).
- Direct packages follow the seven-capability live baseline (decision `0012`).

## High-Risk Areas

- `TemplateBootstrap`, the loading scene, and handle lifetime.
- Public types under `Assets/game.core/`.
- Addressables groups, paths, layout rules, content state and GUID-preserving moves.
- `Packages/manifest.json`, `Packages/packages-lock.json`, and any optional Git
  UPM capability.
- Player identity, define symbols, build scenes, profiles and build pipeline.

## Open Questions

| Question | Owner |
| --- | --- |
| Main target platform: Android, iOS or WebGL | project owner |
| Release profile/CDN matrix beyond the local `Default` profile | project owner |
| Player build argument contract and CI provider | project owner |
| How a derived game refreshes the inherited harness and records its package choices | project owner |
