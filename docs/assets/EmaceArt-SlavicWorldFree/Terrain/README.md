# Terrain

**Prefabs: 2**

Field ground tiles. Every prefab has its own card; see [Prefabs](#prefabs).

| Prefab | Size (m) | Collider |
| --- | --- | --- |
| `Environment_Field_Comp_01a` | no LOD group | none |
| `Terrain_Field_2x2_Seam_01a` | 20.0 | Mesh |

Neither appears in the demo scene. The demo ground is a Unity `Terrain`
(`Terrain/SLAVICA FREE TERRAIN.asset`, outside `Prefabs/`).

## Using it in a map

- The only ground-sized prefab is the 20 m seam field tile, so these two
  prefabs cannot cover a region. Region ground comes from a Unity `Terrain`
  and the tiling textures in `Texture/`.
- `TODO`: read the demo `TerrainData` (size, resolution, terrain layers) with an
  Editor script and record it in the pack README.

## Prefabs

<!-- PREFAB-INDEX:START (generated) -->

**2 prefabs**, one card each.

| Subgroup | Prefab | Size (m) | In scene |
| --- | --- | --- | --- |
| Field | [EA03_Environment_Field_Comp_01a](Field/EA03_Environment_Field_Comp_01a.md) | - | - |
| Field | [EA03_Terrain_Field_2x2_Seam_01a_PRE](Field/EA03_Terrain_Field_2x2_Seam_01a_PRE.md) | 20.0 | - |

<!-- PREFAB-INDEX:END -->
