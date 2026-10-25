# Rooms

Pre-composed interior rooms at the root of `Prefabs/`. 5 prefabs. Table: [prefabs.md](prefabs.md).

| Prefab | Note |
| --- | --- |
| `EA_Room_Comp_3X3` | 3 x 3 room |
| `EA_Room_Int_Kitchen_Comp_7x10_01a` | 7 x 10 kitchen; one in the scene |
| `EA_Room_Int_LivingRoom_Comp_7x7_01a/b/c` | 7 x 7 living rooms; `01a` is in the scene |

They have no LOD group and no root collider; dimensions come from the names.

## Using it in a map

- Interiors are out of scope for the outdoor map pipeline. Keep for later if
  buildings need to be enterable.
- `Rooms` is a name given to the files at the root of `Prefabs/`; it is not a
  folder in the pack.
