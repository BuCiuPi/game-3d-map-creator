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

- Measured runs: plank fence 4.0 m long and 3.0 m tall (dark planks,
  `#1A1815` - `#27221B`); log wall 5.0 m long, 3.6 m tall on a grey stone base
  with a small plank roof; cobble fence 3.8 or 6.3 m long, 1.6 m tall. A region
  border is a line of runs with the end piece closing it.
- Most fences run along X. `Village_Fence_01a` runs along Z (0.3 x 1.8 x 5.2 m),
  so rotate it 90 degrees to line up with the others.
- `RockGate_01a` is a ruined stone arch with a plank roof, 3.7 m tall and 2.2 k
  triangles: a gate landmark, not a repeating piece.
- Fences suit village or farm region borders. They do not read as cliffs or
  natural borders.

## Gaps

- `TODO`: corner pieces. None is named as a corner; check whether the ends work.

## Prefabs

<!-- PREFAB-INDEX:START (generated) -->

**16 prefabs**, one card each.

| Preview | Subgroup | Prefab | W x H x D (m) | Main color | In scene |
| --- | --- | --- | --- | --- | --- |
| <img src="Cobble/EA03_Fence_Cobble_01a_PRE.png" width="64"> | Cobble | [EA03_Fence_Cobble_01a_PRE](Cobble/EA03_Fence_Cobble_01a_PRE.md) | 3.8 x 1.6 x 0.8 | `#4C4D4E` | - |
| <img src="Cobble/EA03_Fence_Cobble_01b_PRE.png" width="64"> | Cobble | [EA03_Fence_Cobble_01b_PRE](Cobble/EA03_Fence_Cobble_01b_PRE.md) | 6.3 x 1.6 x 0.8 | `#494A4B` | - |
| <img src="Cobble/EA03_Fence_Cobble_End_01a_PRE.png" width="64"> | Cobble | [EA03_Fence_Cobble_End_01a_PRE](Cobble/EA03_Fence_Cobble_End_01a_PRE.md) | 1.4 x 2.3 x 1.6 | `#4D4E4F` | - |
| <img src="Plank2/EA03_Fence_Plank_02a_PRE.png" width="64"> | Plank2 | [EA03_Fence_Plank_02a_PRE](Plank2/EA03_Fence_Plank_02a_PRE.md) | 4.0 x 3.0 x 0.3 | `#27221B` | 15 |
| <img src="Plank2/EA03_Fence_Plank_02b_PRE.png" width="64"> | Plank2 | [EA03_Fence_Plank_02b_PRE](Plank2/EA03_Fence_Plank_02b_PRE.md) | 4.0 x 3.1 x 0.4 | `#1A1815` | 6 |
| <img src="Plank2/EA03_Fence_Plank_02c_PRE.png" width="64"> | Plank2 | [EA03_Fence_Plank_02c_PRE](Plank2/EA03_Fence_Plank_02c_PRE.md) | 4.0 x 3.1 x 0.5 | `#1A1815` | 6 |
| <img src="Plank2/EA03_Fence_Plank_02d_PRE.png" width="64"> | Plank2 | [EA03_Fence_Plank_02d_PRE](Plank2/EA03_Fence_Plank_02d_PRE.md) | 1.9 x 3.0 x 0.3 | `#27221B` | 6 |
| <img src="Plank2/EA03_Village_Fence_01a_PRE.png" width="64"> | Plank2 | [EA03_Village_Fence_01a_PRE](Plank2/EA03_Village_Fence_01a_PRE.md) | 0.3 x 1.8 x 5.2 | `#393125` | - |
| <img src="RockGate/EA03_Fence_RockGate_01a_PRE.png" width="64"> | RockGate | [EA03_Fence_RockGate_01a_PRE](RockGate/EA03_Fence_RockGate_01a_PRE.md) | 4.3 x 3.7 x 1.5 | `#494A4B` | - |
| <img src="SolidBeam/EA03_Fence_SolidBeam_End_01a_PRE.png" width="64"> | SolidBeam | [EA03_Fence_SolidBeam_End_01a_PRE](SolidBeam/EA03_Fence_SolidBeam_End_01a_PRE.md) | 6.2 x 3.0 x 3.4 | `#2A231D` | - |
| <img src="VegetablePatch/EA03_Fence_VegetablePatch_02a_PRE.png" width="64"> | VegetablePatch | [EA03_Fence_VegetablePatch_02a_PRE](VegetablePatch/EA03_Fence_VegetablePatch_02a_PRE.md) | 2.6 x 1.0 x 0.4 | `#261E16` | - |
| <img src="Wall/EA03_Fence_WallGate_01a_PRE.png" width="64"> | Wall | [EA03_Fence_WallGate_01a_PRE](Wall/EA03_Fence_WallGate_01a_PRE.md) | 5.0 x 4.9 x 1.8 | `#27231C` | - |
| <img src="Wall/EA03_Fence_Wall_01b_PRE.png" width="64"> | Wall | [EA03_Fence_Wall_01b_PRE](Wall/EA03_Fence_Wall_01b_PRE.md) | 5.0 x 3.6 x 1.8 | `#29241D` | - |
| <img src="Wall/EA03_Fence_Wall_01d_PRE.png" width="64"> | Wall | [EA03_Fence_Wall_01d_PRE](Wall/EA03_Fence_Wall_01d_PRE.md) | 1.5 x 3.5 x 1.6 | `#211D18` | - |
| <img src="rockDesk/EA03_Fence_rockDesk_01g_PRE.png" width="64"> | rockDesk | [EA03_Fence_rockDesk_01g_PRE](rockDesk/EA03_Fence_rockDesk_01g_PRE.md) | 4.1 x 1.8 x 0.7 | `#303030` | - |
| <img src="rockDesk/EA03_Fence_rockDesk_End_01a_PRE.png" width="64"> | rockDesk | [EA03_Fence_rockDesk_End_01a_PRE](rockDesk/EA03_Fence_rockDesk_End_01a_PRE.md) | 0.7 x 1.2 x 0.5 | `#2F2E2D` | - |

<!-- PREFAB-INDEX:END -->
