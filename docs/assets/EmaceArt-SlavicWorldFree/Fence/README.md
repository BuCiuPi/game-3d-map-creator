# Fence

Fences, walls and gates for borders and enclosures. 16 prefabs. Table: [prefabs.md](prefabs.md).

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
