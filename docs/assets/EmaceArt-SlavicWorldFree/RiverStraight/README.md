# RiverStraight

**Prefabs: 4**

Straight river segments. Every prefab has its own card; see [Prefabs](#prefabs).

| Prefab | Size (m) |
| --- | --- |
| `RiverStraight_01f_R` | 20.5 |
| `RiverStraight_01g` | 35.1 |
| `RiverStraight_02a` | 49.6 |
| `RiverStraight_02b` | 58.7 |

All have a mesh collider and LODs. None is used in the demo scene.

## Using it in a map

- Each is one long piece, not a tile. A river is built by chaining pieces
  end to end, so its path must be a sequence of straight runs.
- The name says "straight": there is no bend, junction, source or mouth piece.
  A winding river needs custom meshes or a spline tool.
- `Materials/Water.mat` is the pack's water material, for custom planes.

## Gaps

- No curves, no banks, no waterfall, no lake.
- `TODO`: confirm in the Editor which side of each piece is the connecting
  end and whether the pieces share width.

## Prefabs

<!-- PREFAB-INDEX:START (generated) -->

**4 prefabs**, one card each.

| Subgroup | Prefab | Size (m) | In scene |
| --- | --- | --- | --- |
| - | [EA03_Environment_RiverStraight_01f_R_PRE](EA03_Environment_RiverStraight_01f_R_PRE.md) | 20.5 | - |
| - | [EA03_Environment_RiverStraight_01g_PRE](EA03_Environment_RiverStraight_01g_PRE.md) | 35.1 | - |
| - | [EA03_Environment_RiverStraight_02a_PRE](EA03_Environment_RiverStraight_02a_PRE.md) | 49.6 | - |
| - | [EA03_Environment_RiverStraight_02b_PRE](EA03_Environment_RiverStraight_02b_PRE.md) | 58.7 | - |

<!-- PREFAB-INDEX:END -->
