# Environment

**Prefabs: 41**

Ground-level pieces that are not buildings or plants: roads, flat ground
patches, rocks, bridges, stairs. Every prefab has its own card; see [Prefabs](#prefabs).

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

## Prefabs

<!-- PREFAB-INDEX:START (generated) -->

**41 prefabs**, one card each.

| Subgroup | Prefab | Size (m) | In scene |
| --- | --- | --- | --- |
| - | [EA03_Environment_Road_Cobble_Staris_01b_PRE](EA03_Environment_Road_Cobble_Staris_01b_PRE.md) | 5.1 | - |
| - | [EA03_Prop_House_Door_01b_PRE](EA03_Prop_House_Door_01b_PRE.md) | 2.5 | - |
| - | [EA03_Step_01b_PRE](EA03_Step_01b_PRE.md) | 2.5 | - |
| Bridge | [EA03_Prop_Pier_03d_PRE](Bridge/EA03_Prop_Pier_03d_PRE.md) | 6.6 | - |
| Bridge | [EA03_Prop_Pier_04c_PRE](Bridge/EA03_Prop_Pier_04c_PRE.md) | 4.6 | - |
| Moss | [EA03_Environment_Moss_01_PRE](Moss/EA03_Environment_Moss_01_PRE.md) | 3.3 | - |
| Mud | [EA03_Env_Mud_Flat_01b_PRE](Mud/EA03_Env_Mud_Flat_01b_PRE.md) | 5.7 | - |
| Mud | [EA03_Env_Mud_Flat_01b_x_PRE](Mud/EA03_Env_Mud_Flat_01b_x_PRE.md) | 5.7 | - |
| Mud | [EA03_Env_RoadIslandMud_01a_PRE](Mud/EA03_Env_RoadIslandMud_01a_PRE.md) | 3.9 | - |
| Mud | [EA03_Env_RoadIslandMud_01b_PRE](Mud/EA03_Env_RoadIslandMud_01b_PRE.md) | 3.9 | - |
| Mud | [EA03_Environment_Mud_04b_PRE](Mud/EA03_Environment_Mud_04b_PRE.md) | 5.5 | - |
| Road | [EA03_Env_Road_Wooden_01d_PRE](Road/EA03_Env_Road_Wooden_01d_PRE.md) | 3.1 | - |
| Road | [EA03_Environment_Road_Cobble_01a_PRE](Road/EA03_Environment_Road_Cobble_01a_PRE.md) | 5.2 | 7 |
| Road | [EA03_Environment_Road_Cobble_01b_PRE](Road/EA03_Environment_Road_Cobble_01b_PRE.md) | 1.9 | 2 |
| Road | [EA03_Environment_Road_Cobble_Corner_01a_PRE](Road/EA03_Environment_Road_Cobble_Corner_01a_PRE.md) | 3.7 | 1 |
| Road | [EA03_Village_platform_01a_PRE](Road/EA03_Village_platform_01a_PRE.md) | 5.4 | - |
| RoadIsland | [EA03_Environment_RoadIsland_01_Small_PRE](RoadIsland/EA03_Environment_RoadIsland_01_Small_PRE.md) | 18.7 | - |
| RoadIsland | [EA03_Environment_RoadIsland_08_PRE](RoadIsland/EA03_Environment_RoadIsland_08_PRE.md) | 8.6 | - |
| RoadIsland | [EA03_Environment_RoadIsland_08a_PRE](RoadIsland/EA03_Environment_RoadIsland_08a_PRE.md) | 3.9 | - |
| RoadIsland | [EA03_Environment_RoadIsland_09_PRE](RoadIsland/EA03_Environment_RoadIsland_09_PRE.md) | 18.7 | - |
| RoadIsland | [EA03_Environment_RoadIsland_09a_PRE](RoadIsland/EA03_Environment_RoadIsland_09a_PRE.md) | 18.0 | - |
| RoadSide | [EA03_Environment_RoadSide_PRE](RoadSide/EA03_Environment_RoadSide_PRE.md) | 4.2 | - |
| Rock | [EA03_EnvRock_01a_PRE](Rock/EA03_EnvRock_01a_PRE.md) | 6.6 | - |
| Rock | [EA03_Env_Rock_Slice_01a_PRE](Rock/EA03_Env_Rock_Slice_01a_PRE.md) | 6.1 | - |
| Rock | [EA03_Environment_Rock_BIG_Head_01b_PRE](Rock/EA03_Environment_Rock_BIG_Head_01b_PRE.md) | 1.0 | - |
| Rock | [EA03_Environment_Rock_Big_Head_01a_PRE](Rock/EA03_Environment_Rock_Big_Head_01a_PRE.md) | 0.6 | - |
| Rock | [EA03_Environment_Rock_CatHead_01b_PRE](Rock/EA03_Environment_Rock_CatHead_01b_PRE.md) | 7.1 | - |
| Rock | [EA03_Environment_Rock_Flat_04c_Flat_PRE](Rock/EA03_Environment_Rock_Flat_04c_Flat_PRE.md) | 19.6 | - |
| Rock | [EA03_Environment_Rock_Flat_04c_PRE](Rock/EA03_Environment_Rock_Flat_04c_PRE.md) | 19.6 | - |
| Rock | [EA03_Environment_Rock_Mini_CatHead_01a_PRE](Rock/EA03_Environment_Rock_Mini_CatHead_01a_PRE.md) | 4.7 | - |
| Rock | [EA03_Environment_Rock_Mini_Head_01a_PRE](Rock/EA03_Environment_Rock_Mini_Head_01a_PRE.md) | 1.5 | 45 |
| Rock | [EA03_Nature_Solid_Flat_01a_PRE](Rock/EA03_Nature_Solid_Flat_01a_PRE.md) | - | - |
| Rock | [EA03_Nature_Solid_Flat_01b_PRE](Rock/EA03_Nature_Solid_Flat_01b_PRE.md) | 16.4 | - |
| Rock | [EA03_Nature_Solid_Flat_01c_PRE](Rock/EA03_Nature_Solid_Flat_01c_PRE.md) | 16.4 | - |
| Sand | [EA03_Env_Mud_Flat_01c_Half_PRE](Sand/EA03_Env_Mud_Flat_01c_Half_PRE.md) | 8.1 | - |
| Sand | [EA03_Env_Mud_Flat_01c_PRE](Sand/EA03_Env_Mud_Flat_01c_PRE.md) | 8.3 | - |
| Sand | [EA03_Env_Sand_Flat_01a_PRE](Sand/EA03_Env_Sand_Flat_01a_PRE.md) | 4.8 | - |
| Sand | [EA03_Env_Sand_Flat_01b_PRE](Sand/EA03_Env_Sand_Flat_01b_PRE.md) | 12.8 | - |
| Sand | [EA03_Environment_Sand_01b_PRE](Sand/EA03_Environment_Sand_01b_PRE.md) | 8.9 | - |
| Sand | [EA03_Nature_Bush_Flat_03e_PRE](Sand/EA03_Nature_Bush_Flat_03e_PRE.md) | 19.6 | - |
| Stairs | [EA03_Village_step_platform_Stair_R_03a_PRE](Stairs/EA03_Village_step_platform_Stair_R_03a_PRE.md) | 5.2 | - |

<!-- PREFAB-INDEX:END -->
