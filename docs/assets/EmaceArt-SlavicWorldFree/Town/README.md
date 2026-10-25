# Town

**Prefabs: 10**

Town buildings. Every prefab has its own card; see [Prefabs](#prefabs).

| Subgroup | Count | Size (m) | Note |
| --- | --- | --- | --- |
| `Administrative` | 3 | 21.0 | one is a modular variant; one instance of `01b_Modular` is in the scene |
| `Building` | 5 | no LOD group | `Town_House_Comp_*` houses assembled from nested `Compound` parts |
| `Compound/Walls_Painted` | 2 | 8.5 | painted wall parts |

## Using it in a map

- `Town_House_Comp_*` are whole houses built from nested prefabs (foundation,
  walls, porch, roof; see each card's Nested prefabs row). The root has no LOD
  group or collider of its own; the parts carry them. They are the worked
  examples of how `Compound` parts snap together.
- The administrative buildings are about 21 m, the largest finished buildings
  besides the 24.5 m shed in `Village`.

## Gaps

- `TODO`: sizes for `Building`, and whether these share the atlas material.

## Prefabs

<!-- PREFAB-INDEX:START (generated) -->

**10 prefabs**, one card each.

| Subgroup | Prefab | Size (m) | In scene |
| --- | --- | --- | --- |
| Administrative | [EA03_Town_Building_Administrative _01a_PRE](Administrative/EA03_Town_Building_Administrative%20_01a_PRE.md) | 21.0 | - |
| Administrative | [EA03_Town_Building_Administrative _01b_Modular_PRE](Administrative/EA03_Town_Building_Administrative%20_01b_Modular_PRE.md) | 21.0 | 1 |
| Administrative | [EA03_Town_Building_Administrative _01c_PRE](Administrative/EA03_Town_Building_Administrative%20_01c_PRE.md) | - | - |
| Building | [EA03_Town_House_Comp_01a_PRE](Building/EA03_Town_House_Comp_01a_PRE.md) | - | - |
| Building | [EA03_Town_House_Comp_02a_PRE](Building/EA03_Town_House_Comp_02a_PRE.md) | - | - |
| Building | [EA03_Town_House_Comp_03a_PRE](Building/EA03_Town_House_Comp_03a_PRE.md) | - | - |
| Building | [EA03_Town_House_Comp_03b_PRE](Building/EA03_Town_House_Comp_03b_PRE.md) | - | - |
| Building | [EA03_Town_House_Comp_03c_PRE](Building/EA03_Town_House_Comp_03c_PRE.md) | - | - |
| Compound/Walls_Painted | [EA03_Town_Hut_Wall_Painted_Desk_02a_PRE](Compound/Walls_Painted/EA03_Town_Hut_Wall_Painted_Desk_02a_PRE.md) | 8.5 | - |
| Compound/Walls_Painted | [EA03_Village_Hut_Wall_Desk_02a_PRE](Compound/Walls_Painted/EA03_Village_Hut_Wall_Desk_02a_PRE.md) | 8.5 | - |

<!-- PREFAB-INDEX:END -->
