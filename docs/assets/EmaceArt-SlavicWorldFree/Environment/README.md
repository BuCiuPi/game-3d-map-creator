# Environment

Ground-level pieces that are not buildings or plants: roads, flat ground
patches, rocks, bridges, stairs. 41 prefabs. Table: [prefabs.md](prefabs.md).

| Subgroup | Count | Size (m) | What it is |
| --- | --- | --- | --- |
| `Road` | 5 | 1.9 - 5.4 | cobble road tiles (5.2 m main tile, 1.9 m short), a corner, a wooden road, a platform |
| `RoadIsland` | 5 | 3.9 - 18.7 | irregular road patches; the two 18 m ones are the largest |
| `RoadSide` | 1 | 4.2 | road edge piece |
| `Sand` | 6 | 4.8 - 19.6 | flat sand/mud/bush patches laid on the ground |
| `Mud` | 5 | 3.9 - 5.7 | flat mud patches and road-island mud |
| `Rock` | 12 | 0.6 - 19.6 | small heads to a 19.6 m formation; 3 have no collider |
| `Bridge` | 2 | 4.6 - 6.6 | piers |
| `Stairs` | 1 | 5.2 | village platform stair |
| `Moss` | 1 | 3.3 | moss overlay, no collider |
| (root) | 3 | 2.5 - 5.1 | a cobble stair, a door, a step |

## Using it in a map

- Roads are tiles, so a path layer can be laid as a chain of 5.2 m cobble tiles
  with the corner piece at turns. Only 10 road instances appear in the demo
  scene, so there is little evidence of larger road networks.
- Sand, mud and road-island pieces are flat patches, not region-filling ground.
  They suit transitions or clearings, not a whole region.
- Rocks are the only cliff-like assets. `Rock` pieces over 15 m are the
  candidates for border rims; confirm their shape in the Editor first.
  Only `Rock_Mini_Head_01a` is used in the scene (45 instances).

## Gaps

- No cliff-wall module, no sea or lake, no beach edge.
- `TODO`: check whether `Sand` and `Mud` patches have a matching edge fade, by
  viewing them in the Editor.
