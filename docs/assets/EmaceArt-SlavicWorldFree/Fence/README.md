# Fence

**Prefabs: 16**

Fences, walls and gates for borders and enclosures. Every prefab has its own card; see [Prefabs](#prefabs).

| Subgroup | Count | Size (m) | What it is |
| --- | --- | --- | --- |
| `Plank2` | 5 | 3.0 - 5.2 | plank fence; a 4 m main run, 3 m end |
| `Cobble` | 3 | 2.3 - 6.3 | stone-cobble fence and end piece |
| `Wall` | 3 | 3.5 - 5.0 | wall runs and a gate |
| `rockDesk` | 2 | 1.2 - 4.1 | low stone fence and end |
| `RockGate` | 1 | 4.3 | stone gate |
| `SolidBeam` | 1 | 6.2 | beam fence end |
| `VegetablePatch` | 1 | 2.6 | garden fence |

All have mesh colliders. In the demo scene only `Plank2` is used, 33 instances.

## Using it in a map

- Runs repeat at about 4 m, so a region border can be a fence line with the
  end piece closing it. Sizes are LOD bounds; confirm the true snap length.
- Fences suit village or farm region borders. They do not read as cliffs or
  natural borders.

## Gaps

- `TODO`: corner pieces. None is named as a corner; check whether the ends work.

## Prefabs

<!-- PREFAB-INDEX:START (generated) -->

**16 prefabs**, one card each.

| Subgroup | Prefab | Size (m) | In scene |
| --- | --- | --- | --- |
| Cobble | [EA03_Fence_Cobble_01a_PRE](Cobble/EA03_Fence_Cobble_01a_PRE.md) | 3.8 | - |
| Cobble | [EA03_Fence_Cobble_01b_PRE](Cobble/EA03_Fence_Cobble_01b_PRE.md) | 6.3 | - |
| Cobble | [EA03_Fence_Cobble_End_01a_PRE](Cobble/EA03_Fence_Cobble_End_01a_PRE.md) | 2.3 | - |
| Plank2 | [EA03_Fence_Plank_02a_PRE](Plank2/EA03_Fence_Plank_02a_PRE.md) | 4.0 | 15 |
| Plank2 | [EA03_Fence_Plank_02b_PRE](Plank2/EA03_Fence_Plank_02b_PRE.md) | 4.0 | 6 |
| Plank2 | [EA03_Fence_Plank_02c_PRE](Plank2/EA03_Fence_Plank_02c_PRE.md) | 4.0 | 6 |
| Plank2 | [EA03_Fence_Plank_02d_PRE](Plank2/EA03_Fence_Plank_02d_PRE.md) | 3.0 | 6 |
| Plank2 | [EA03_Village_Fence_01a_PRE](Plank2/EA03_Village_Fence_01a_PRE.md) | 5.2 | - |
| RockGate | [EA03_Fence_RockGate_01a_PRE](RockGate/EA03_Fence_RockGate_01a_PRE.md) | 4.3 | - |
| SolidBeam | [EA03_Fence_SolidBeam_End_01a_PRE](SolidBeam/EA03_Fence_SolidBeam_End_01a_PRE.md) | 6.2 | - |
| VegetablePatch | [EA03_Fence_VegetablePatch_02a_PRE](VegetablePatch/EA03_Fence_VegetablePatch_02a_PRE.md) | 2.6 | - |
| Wall | [EA03_Fence_WallGate_01a_PRE](Wall/EA03_Fence_WallGate_01a_PRE.md) | 5.0 | - |
| Wall | [EA03_Fence_Wall_01b_PRE](Wall/EA03_Fence_Wall_01b_PRE.md) | 5.0 | - |
| Wall | [EA03_Fence_Wall_01d_PRE](Wall/EA03_Fence_Wall_01d_PRE.md) | 3.5 | - |
| rockDesk | [EA03_Fence_rockDesk_01g_PRE](rockDesk/EA03_Fence_rockDesk_01g_PRE.md) | 4.1 | - |
| rockDesk | [EA03_Fence_rockDesk_End_01a_PRE](rockDesk/EA03_Fence_rockDesk_End_01a_PRE.md) | 1.2 | - |

<!-- PREFAB-INDEX:END -->
