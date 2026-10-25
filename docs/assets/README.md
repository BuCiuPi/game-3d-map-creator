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
      <Subgroup>/<Prefab>.png  384 px rendered preview
```

A prefab card has its preview, a generated table (path, GUID, collider, LODs,
meshes, materials, nested prefabs, demo-scene count, measured bounds, pivot,
triangles, average and dominant colors) and a hand-written `## Notes` section
that regeneration keeps.

## Regenerating

Two steps. The first needs Unity closed on this project.

1. Render and measure in Unity batch mode (writes the PNGs and a metrics JSON):

   ```text
   Unity.exe -batchmode -projectPath . -executeMethod Game.AssetDocs.Editor.PrefabInspector.Run
     -packRoot "<pack-root>" -out "docs/assets/<Pack>" -metrics "Logs/prefab-metrics.json"
     -logFile Logs/inspect.log
   ```

   Do not point `-metrics` at `Temp/`: Unity deletes it on exit.
2. Write the cards and indexes:
   `python -I tools/assets/gen_prefab_tables.py "<pack-root>" docs/assets/<Pack> "<scene>" --metrics Logs/prefab-metrics.json`

The inspector lives in `Assets/game.assetdocs/Editor/PrefabInspector.cs`.
Colors are sampled from the rendered preview, so they include the preview
lighting; compare them with each other, not with albedo values.

## Packs

| Pack | Source folder | Prefabs |
| --- | --- | --- |
| [EmaceArt-SlavicWorldFree](EmaceArt-SlavicWorldFree/README.md) | `Assets/EmaceArt/Slavic World Free/` | 238 |

## Rules

- Hand-written text lives in a README above the `PREFAB-INDEX` markers and in a
  card below `## Notes`. Everything else is generated; never edit it.
- A claim cites what it came from: a prefab card, a scene, or an Editor check.
  Unverified items are `TODO`.
- Adding a pack: create `docs/assets/<Pack>/`, run the generator, then write the
  pack README and one README per category.
- Pipeline context (how prefabs get placed from a map) lives in
  `docs/map-to-environment/`, not here.
