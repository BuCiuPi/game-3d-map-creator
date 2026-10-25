# Nature

**Prefabs: 34**

Vegetation: trees, bushes, plants, grass. Every prefab has its own card; see [Prefabs](#prefabs).

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

## Prefabs

<!-- PREFAB-INDEX:START (generated) -->

**34 prefabs**, one card each.

| Subgroup | Prefab | Size (m) | In scene |
| --- | --- | --- | --- |
| Bushes | [EA03_Nature_Bush_01a_PRE](Bushes/EA03_Nature_Bush_01a_PRE.md) | 3.6 | - |
| Bushes | [EA03_Nature_Bush_01b_PRE](Bushes/EA03_Nature_Bush_01b_PRE.md) | 4.0 | - |
| Bushes | [EA03_Nature_Bush_01c_PRE](Bushes/EA03_Nature_Bush_01c_PRE.md) | 4.0 | - |
| Bushes | [EA03_Nature_Bush_01d_PRE](Bushes/EA03_Nature_Bush_01d_PRE.md) | 3.6 | - |
| Bushes | [EA03_Nature_Bush_02a_PRE](Bushes/EA03_Nature_Bush_02a_PRE.md) | 3.7 | - |
| Bushes | [EA03_Nature_Bush_02b_PRE](Bushes/EA03_Nature_Bush_02b_PRE.md) | 3.7 | - |
| Bushes | [EA03_Nature_Bush_03a_PRE](Bushes/EA03_Nature_Bush_03a_PRE.md) | 2.8 | - |
| Bushes | [EA03_Nature_Bush_03b_PRE](Bushes/EA03_Nature_Bush_03b_PRE.md) | 3.4 | - |
| Bushes | [EA03_Nature_Bush_03c_PRE](Bushes/EA03_Nature_Bush_03c_PRE.md) | 1.9 | - |
| Bushes | [EA03_Nature_Bush_03d_PRE](Bushes/EA03_Nature_Bush_03d_PRE.md) | 4.5 | - |
| Bushes | [EA03_Nature_Bush_04a_PRE](Bushes/EA03_Nature_Bush_04a_PRE.md) | 2.5 | - |
| Bushes | [EA03_Nature_Bush_05a_PRE](Bushes/EA03_Nature_Bush_05a_PRE.md) | 5.9 | - |
| Bushes | [EA03_Nature_Bush_05b_PRE](Bushes/EA03_Nature_Bush_05b_PRE.md) | 5.9 | - |
| Bushes | [EA03_Nature_Bush_Flat_03d_PRE](Bushes/EA03_Nature_Bush_Flat_03d_PRE.md) | 5.1 | - |
| Grass | [EA03_Plant_Grass_01c_PRE](Grass/EA03_Plant_Grass_01c_PRE.md) | 0.9 | - |
| Grass | [EA03_Plant_Grass_01d_PRE](Grass/EA03_Plant_Grass_01d_PRE.md) | 0.8 | - |
| Grass | [EA03_Plant_Grass_01e_PRE](Grass/EA03_Plant_Grass_01e_PRE.md) | 1.7 | 344 |
| Grass | [EA03_Plant_Grass_02a_PRE](Grass/EA03_Plant_Grass_02a_PRE.md) | 0.5 | 10389 |
| Plant | [EA03_Nature_SweetFlag_01a_PRE](Plant/EA03_Nature_SweetFlag_01a_PRE.md) | 3.0 | - |
| Plant | [EA03_Nature_SweetFlag_01b_PRE](Plant/EA03_Nature_SweetFlag_01b_PRE.md) | 3.4 | - |
| Plant | [EA03_Nature_SweetFlag_01c_PRE](Plant/EA03_Nature_SweetFlag_01c_PRE.md) | 3.0 | - |
| Plant | [EA03_Nature_SweetFlag_01d_PRE](Plant/EA03_Nature_SweetFlag_01d_PRE.md) | 3.0 | - |
| Plant | [EA03_Nature_SweetFlag_01e_PRE](Plant/EA03_Nature_SweetFlag_01e_PRE.md) | 2.8 | - |
| Plant | [EA03_Nature_SweetFlag_02a_PRE](Plant/EA03_Nature_SweetFlag_02a_PRE.md) | 3.0 | - |
| Plant | [EA03_Nature_SweetFlag_02c_PRE](Plant/EA03_Nature_SweetFlag_02c_PRE.md) | 3.0 | - |
| Plant | [EA03_Nature_SweetFlag_02d_PRE](Plant/EA03_Nature_SweetFlag_02d_PRE.md) | 2.3 | - |
| Plant | [EA03_Nature_TrunkPlant_01a_PRE](Plant/EA03_Nature_TrunkPlant_01a_PRE.md) | 4.4 | - |
| Tree | [EA03_Nature_Tree_01b_PRE](Tree/EA03_Nature_Tree_01b_PRE.md) | 10.3 | 107 |
| Tree | [EA03_Nature_Tree_02b_PRE](Tree/EA03_Nature_Tree_02b_PRE.md) | 11.5 | - |
| Tree | [EA03_Nature_Tree_02c_PRE](Tree/EA03_Nature_Tree_02c_PRE.md) | 9.4 | - |
| Tree | [EA03_Nature_Tree_03b_PRE](Tree/EA03_Nature_Tree_03b_PRE.md) | 9.3 | 1 |
| Tree | [EA03_Nature_Tree_03c_PRE](Tree/EA03_Nature_Tree_03c_PRE.md) | 11.8 | - |
| Tree | [EA03_Nature_Tree_05a_PRE](Tree/EA03_Nature_Tree_05a_PRE.md) | 11.8 | - |
| Tree | [EA03_Nature_Tree_06b_PRE](Tree/EA03_Nature_Tree_06b_PRE.md) | 16.8 | - |

<!-- PREFAB-INDEX:END -->
