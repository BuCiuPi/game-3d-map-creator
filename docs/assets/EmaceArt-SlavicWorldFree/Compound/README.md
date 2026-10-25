# Compound

Modular hut parts: foundation, walls, roofs, porch, balcony. 24 prefabs. Table: [prefabs.md](prefabs.md).

| Subgroup | Count | Size (m) | Collider |
| --- | --- | --- | --- |
| `Foundation` | 1 | 20.1 | Box |
| `Walls` | 8 | 5.3 - 8.9 | 4 Box, 4 Mesh |
| `Walls_Painted` | 3 | 8.6 - 20.0 | 2 Mesh, 1 Box |
| `Roofs` | 10 | 8.5 - 20.0 | Mesh |
| `Porch` | 1 | 6.4 | Mesh |
| `Balcony` | 1 | 4.7 | Mesh |

None of these is used in the demo scene as parts; it uses finished or
single-piece buildings from `Village` and `Town`.

## Using it in a map

- Parts snap together to make a hut. They are for building variety, not for
  filling a region. A single 20 m foundation implies huts of roughly that size.
- For a first pass, use the finished buildings in `Village` and `Town` and
  leave modular assembly for later.

## Gaps

- `TODO`: the snapping rules (pivot and connection points) are not documented.
  Assemble one hut in the Editor and record the offsets.
