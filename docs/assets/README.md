# Asset Knowledge

What each imported 3D asset pack contains, written so a map builder can pick
prefabs without opening the Editor. This repo has one Unity project, so the
levels are:

```text
docs/assets/
  <Pack>/                      one imported pack (the unit you buy or import)
    README.md                  pack overview, rendering notes, gaps
    <Category>/                first folder under the pack's Prefabs/
      README.md                hand-written: role, subgroups, usage, gaps
      prefabs.md               generated: one row per prefab
```

## Packs

| Pack | Source folder | Prefabs |
| --- | --- | --- |
| [EmaceArt-SlavicWorldFree](EmaceArt-SlavicWorldFree/README.md) | `Assets/EmaceArt/Slavic World Free/` | 238 |

## Rules

- `README.md` files are hand-written and carry judgment. `prefabs.md` files are
  generated; never edit them. Regenerate with
  `python -I tools/assets/gen_prefab_tables.py "<pack-root>" docs/assets/<Pack> "<scene>"`.
- A claim in a README cites what it came from: a `prefabs.md` row, a scene, or
  an Editor check. Unverified items are `TODO`.
- Adding a pack: create `docs/assets/<Pack>/`, run the generator, then write the
  pack README and one README per category.
- Pipeline context (how prefabs get placed from a map) lives in
  `docs/map-to-environment/`, not here.
