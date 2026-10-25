# Overview

## Current Behavior

The project requires two private submodules. Legacy Jigsaw assemblies consume
their game, common, and VFX packages. A fresh clone cannot resolve the local
package paths until both gitlinks are initialized.

## Target Behavior

The template owns a minimal data core, neutral bootstrap, and UI shell in
`Assets/`. It contains no internal package or submodule by default. Projects add
networking through pinned Git UPM dependencies only when a feature needs it.

## Affected Surfaces

- `Assets/game.shared/`, `Assets/game.visualization/`, bootstrap and UI
- `Assets/AddressableAssetsData/` and serialized package asset GUIDs
- `Packages/manifest.json`, `Packages/packages-lock.json`, `.gitmodules`
- `Packages/game-packages`, `Packages/vfx-packages`

## Affected Docs

- `docs/decisions/0008-no-default-internal-packages.md`
- `artifacts/unity-template-package-exit-plan.html`
- `.claude/knowledge/01-project-brief.md` when the migration completes

## Non-Goals

- Add networking, backend, analytics, ads, or a game feature.
- Copy Cricket, HyperEel, or `com.paul.*` package implementations.
- Complete the broader Addressables modernization in the package-removal commit.
