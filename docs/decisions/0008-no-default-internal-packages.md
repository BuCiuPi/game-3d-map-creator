# 0008 No Default Internal Packages

Date: 2026-10-25

## Status

Accepted (owner, 2026-10-25); implemented by `59a0465`.

## Context

Before this decision was implemented, the template mounted
`Packages/game-packages` and `Packages/vfx-packages` as private submodules and
Jigsaw runtime/assets consumed their game and VFX assemblies.

The template must be a thin, bootable starting point. A fresh clone should not
need access to private repositories or carry game, VFX, networking, or backend
capabilities that the new project does not use.

## Decision

1. Remove both submodules after their C#, serialized GUID, and Addressables
   consumers reach zero.
2. Remove `paul.game.*`, `com.paul.vfx.*`, and `com.paul.common.*` from the
   template manifest. Do not copy their source into the main repository.
3. Keep template-owned data primitives in `Assets/game.core/`; implement only
   the neutral bootstrap and UI shell capabilities needed to boot the template.
4. Do not install HyperEel packages by default.
5. When a project adds its first real API feature, add only the required
   networking/common packages through Git UPM, pinned to a commit SHA. Declare
   every required Git package in the project manifest because Unity does not
   support transitive Git dependencies between packages.

Execution order and gates live in
`artifacts/unity-template-package-exit-plan.html`.
The API package recipe lives in
`docs/architecture/networking-on-demand.md`.

## Consequences

- New projects clone and compile without recursive submodule initialization.
- Package removal occurred only after code, asset, GUID and Addressables
  consumers reached zero.
- Networking and other shared capabilities become explicit project choices.
- Several packages from one Git monorepo may cause duplicate clones; use one
  self-contained package or a private registry if that becomes material.

## Alternatives Considered

1. Keep both submodules: rejected because every new project pays setup and
   dependency cost for unused game/VFX modules.
2. Keep only the current common package folders as a submodule: rejected as the
   default because `Assets/game.core/` does not need them.
3. Copy package source into `Assets/`: rejected; the template learns contracts,
   not package implementation.
