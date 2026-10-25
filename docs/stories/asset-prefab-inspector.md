# Asset Prefab Inspector

## Status

implemented

## Lane

normal (intake `2026-10-25 16:50`)

## Behavior

An Editor-only batch tool renders a preview and measures every prefab of an
imported asset pack, and the docs generator turns the result into one knowledge
card per prefab under `docs/assets/<Pack>/`. A map builder can see what a
prefab looks like, how big it is and where its pivot sits without opening Unity.

## Relevant Docs

- `docs/assets/README.md`: the tree layout and the two regeneration steps
- `docs/assets/EmaceArt-SlavicWorldFree/README.md`: the first pack, written from the output
- `docs/map-to-environment/README.md`: the pipeline these cards feed

## Acceptance Criteria

- `PrefabInspector.Run` in batch mode exits 0 and writes one PNG and one metrics
  entry per prefab under the pack's `Prefabs/`.
- Cards and category indexes are generated from the metrics; hand-written text
  (README above the index markers, card below `## Notes`) survives regeneration.
- Running the generator twice gives no diff.

## Surfaces Touched

- [x] What belongs in the template (adding or removing a module): new
      Editor-only assembly `game.assetdocs.editor`; nothing ships in a player.

## Design Notes

- `Assets/game.assetdocs/Editor/PrefabInspector.cs`: instantiates each prefab
  in an empty scene, forces LOD0, measures LOD0 renderer bounds, triangles and
  vertices, renders 384 px with a 30-degree camera from one direction under one
  key light, and keeps the top three rendered color bins.
- Colors come from the rendered image, so they include lighting.
- `tools/assets/gen_prefab_tables.py --metrics` merges the JSON into the cards.

## Validation

| Proof | Expected | Result |
| --- | --- | --- |
| `compile` | no `error CS` in the batch log | `Logs/inspect.log`: 0 `error CS` |
| tool run | 238 prefabs, 238 PNGs, exit 0 | exit 0; "PrefabInspector: 238 prefabs"; 238 PNGs under `docs/assets/EmaceArt-SlavicWorldFree/` |
| `editor-evidence` | previews render with URP materials | 38 previews checked by eye across Environment, Nature, River, Terrain, Fence, Village, Town; none pink or empty |
| idempotence | second generator run: no diff | run twice; no change |

## Harness Delta

`docs/assets/` gained the preview and measurement workflow. Category READMEs
were rewritten from the previews; several name-based assumptions were wrong
(river pieces are banks, "Big Head" rocks are under 1 m).

## Evidence

See the Validation table and the `docs/TRACE_LOG.md` row citing `2026-10-25 16:50`.
