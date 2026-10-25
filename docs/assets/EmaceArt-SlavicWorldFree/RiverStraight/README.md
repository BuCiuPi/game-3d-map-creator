# RiverStraight

Straight river segments. 4 prefabs. Table: [prefabs.md](prefabs.md).

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
