# Asset Knowledge

What each imported 3D asset pack contains, written so a map builder can pick
prefabs without opening the Editor. This repo has one Unity project, so the
levels are:

```text
docs/assets/
  <Pack>/                      one imported pack (the unit you buy or import)
    README.md                  pack overview, rendering notes, gaps
    <Category>/                first folder under the pack's Prefabs/
      README.md                prefab count, role, subgroups, usage, gaps;
                               ends with a generated prefab list
      <Subgroup>/<Prefab>.md   one card per prefab, mirroring Prefabs/
```

A prefab card has a generated table (path, GUID, size, collider, LODs, meshes,
materials, nested prefabs, demo-scene count) and a hand-written `## Notes`
section that regeneration keeps.

## Packs

| Pack | Source folder | Prefabs |
| --- | --- | --- |
| [EmaceArt-SlavicWorldFree](EmaceArt-SlavicWorldFree/README.md) | `Assets/EmaceArt/Slavic World Free/` | 238 |

## Rules

- Hand-written text lives in a README above the `PREFAB-INDEX` markers and in a
  card below `## Notes`. Everything else is generated; never edit it. Regenerate with
  `python -I tools/assets/gen_prefab_tables.py "<pack-root>" docs/assets/<Pack> "<scene>"`.
- A claim cites what it came from: a prefab card, a scene, or an Editor check.
  Unverified items are `TODO`.
- Adding a pack: create `docs/assets/<Pack>/`, run the generator, then write the
  pack README and one README per category.
- Pipeline context (how prefabs get placed from a map) lives in
  `docs/map-to-environment/`, not here.
