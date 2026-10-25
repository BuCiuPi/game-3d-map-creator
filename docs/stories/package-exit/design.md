# Design

## Data / Models

`Assets/game.core/` remains the owner of `DataAsset<TData>` and
`DataTable<TRow>`. This migration does not change their public API.

## Flow And Wiring

Replace the Jigsaw bootstrap/lobby dependency chain with a neutral composition
root and UI shell before deleting its package-backed consumers. The final boot
path has no auth, API, ads, analytics, shop, quest, chapter, or gameplay work.

## Contract Surface

Template-owned contracts are limited to boot and shell UI. Networking contracts
arrive with the project feature that consumes them, through Git UPM packages
pinned by commit SHA.

## Assets / Resources

Boot loading and fatal recovery prefabs belong under
`Assets/game.assetbundles/00_local/ui/shell/`; they are wired directly into the
bootstrap scene so failure UI does not depend on Addressables instantiation.
Toast, generic MessageOverlay, and the global shell belong under
`Assets/game.assetbundles/01_init/ui/00_shared/`. Every package asset GUID
reference must be classified before deletion. SmartAddresser remains the owner
of group membership.

## Build Configurations

Proof uses Unity `6000.3.10f1` in the current Editor target. Device/platform
proof remains deferred until the owner selects the template's main platform.

## Alternatives Considered

1. Preserve the legacy lobby as the template shell: rejected because it keeps
   game and package coupling.
2. Keep common packages preinstalled: rejected by decision `0008`.
3. Add networking during cleanup: rejected as a speculative dependency.
