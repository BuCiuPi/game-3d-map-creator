# Town

Town buildings. 10 prefabs. Table: [prefabs.md](prefabs.md).

| Subgroup | Count | Size (m) | Note |
| --- | --- | --- | --- |
| `Administrative` | 3 | 21.0 | one is a modular variant; one instance of `01b_Modular` is in the scene |
| `Building` | 5 | no LOD group | `Town_House_Comp_*` houses; no collider on the root |
| `Compound/Walls_Painted` | 2 | 8.5 | painted wall parts |

## Using it in a map

- `Town_House_Comp_*` are whole houses, but the root has no LOD group and no
  collider, so they cannot be size-checked from the prefab. Measure them in the
  Editor and add the collider if they need to block movement.
- The administrative buildings are about 21 m, the largest finished buildings
  besides the 24.5 m shed in `Village`.

## Gaps

- `TODO`: sizes for `Building`, and whether these share the atlas material.
