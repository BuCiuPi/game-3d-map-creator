# Template Core Data

The template provides two local ScriptableObject primitives under `Game.Core.Runtime.Data`. They reproduce only the useful capability boundary learned from HyperEel common core; no HyperEel, YGG, Cricket, or package-module source/API is copied.

## DA: one typed aggregate

Use `DataAsset<TData>` when one asset represents one cohesive configuration, catalog, route map, or presentation profile.

- Concrete type: `XxxDataAsset`
- Asset filename: `DA_Xxx.asset`
- Runtime access: read-only through `Data`
- Editor importers may replace the payload through editor-only APIs

DA does not mean “always loaded” or “initial data.” It is a data shape. Delivery is chosen separately from the earliest legitimate consumer.

## DT: homogeneous rows with stable IDs

Use `DataTable<TRow>` when one asset contains repeated rows of the same schema and consumers need lookup by a stable string ID.

- Concrete type: `XxxDataTable`
- Row type: `XxxDataTableRow : DataTableRow`
- Asset filename: `DT_Xxx.asset`
- Runtime access: `TryGetRow`, `GetRequiredRow`, or the read-only `Rows` list
- Validation rejects null rows, empty IDs, and duplicate IDs
- The ID index is created lazily and invalidated after Editor mutation

DT is not a replacement for runtime server state or arbitrary dictionaries. It is authored, immutable reference data.

## Addressables placement

Place each concrete asset by availability and ownership, not by the `DA_`/`DT_` prefix:

```text
Assets/game.assetbundles/
  00_local/
    data/bootstrap/assets/       # only boot-critical DA assets
    data/bootstrap/tables/       # only boot-critical DT assets
  01_init/
    shared/data/assets/          # shared DA assets needed after initialization
    shared/data/tables/          # shared DT assets needed after initialization
  02_partial/
    <feature>/data/assets/       # feature-owned DA assets
    <feature>/data/tables/       # feature-owned DT assets
```

Data required before Addressables initialization must be serialized directly into the bootstrap scene/object. It must not create a circular dependency by trying to load itself through Addressables.

The scope that loads an asset owns its Addressables handle. DT/DA objects never release themselves:

- application scope owns `01_init` data;
- feature scope owns `02_partial/<feature>` data;
- cancel, failure, and disposal paths release the acquired handle.

The template intentionally contains no fake gameplay/economy/quest DT or DA assets. Concrete assets are added only when a real core or feature consumer exists.
