# Prop

Standalone objects for scatter and interiors: containers, furniture, tools. 54 prefabs. Table: [prefabs.md](prefabs.md).

| Subgroup | Count | Size (m) | What it is |
| --- | --- | --- | --- |
| `Container` | 14 | 0.3 - 1.7 | bags, bales, barrels, baskets, crates, chests; the most used in the scene (46) |
| `Furniture` | 14 | 0.5 - 3.7 | shelves, stools, tables, bench, bed; mostly Box colliders |
| `wooden` | 5 | 0.8 - 2.3 | wood pieces, pins, a withy fence |
| `Village` | 4 | 2.0 - 5.6 | stove, stand, ladder, wheel |
| `Carpentry` | 3 | 1.2 - 1.7 | brushwood, stump, desk set |
| `Hunter` | 3 | 1.3 - 1.5 | animal skins and a trophy |
| `Forester` | 2 | 2.2 - 17.3 | cut-out trees (17.3 m) and a wooden piece |
| `Sign` | 1 | 4.0 | chapel sign |
| (root) | 8 | 0.2 - 0.3 | food items such as apples and carrots |

## Using it in a map

- Container and Carpentry props are the outdoor scatter set for a village
  region: bales, baskets, brushwood near huts.
- Furniture, Hunter and the root food items are interior dressing and belong
  with `Rooms` and `Items`, not in open ground scatter.
- `Forester/..CutOutTrees` is 17.3 m, much larger than other props; treat it as
  a landmark, not scatter.

## Gaps

- No wilderness props (campfire, tent, signpost besides the chapel sign).
