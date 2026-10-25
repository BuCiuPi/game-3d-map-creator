# Nature

Vegetation: trees, bushes, plants, grass. 34 prefabs. Table: [prefabs.md](prefabs.md).

| Subgroup | Count | Size (m) | Collider | What it is |
| --- | --- | --- | --- | --- |
| `Tree` | 7 | 9.3 - 16.8 | 6 Mesh, 1 Box | trees; `Tree_06b` is the tallest |
| `Bushes` | 14 | 1.9 - 5.9 | mostly none | bushes in lettered variants |
| `Plant` | 9 | 2.3 - 4.4 | none | plants such as the SweetFlag set |
| `Grass` | 4 | 0.5 - 1.7 | none | grass tufts |

## Using it in a map

- This is the strongest category for forest and meadow regions.
- The demo scene is 10,733 grass instances and 108 trees (almost all
  `Nature_Tree_01b`). Grass is placed as tens of thousands of instances, which
  is only reasonable as terrain detail or GPU instancing, not as game objects.
- Only `Tree_01b` and `Tree_03b` of the 7 trees appear in the scene. The others
  are unchecked in context, so view them before using them for a region.
- Bushes and plants have no collider, so they never block movement. Trees do.

## Gaps

- No dead trees, palms or conifers distinguishable by name; `TODO`: view the
  trees in the Editor and tag each for a biome.
