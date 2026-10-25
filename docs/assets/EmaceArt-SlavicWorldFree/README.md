# EmaceArt: Slavic World Free

Pack overview. Source: `Assets/EmaceArt/Slavic World Free/`. Facts read from the
prefab and scene YAML on 2026-10-25; per-prefab tables are generated (see
`../README.md`).

## What it is

A low-poly Slavic village and forest kit, a free sample of a larger paid pack
(`Thanks and checkout this/Info.txt`). It was built for Unity 2019 Standard
render; `URP_Support/` holds the URP setup. The project is on URP 17.

| Folder | Files (non-.meta) | Use |
| --- | --- | --- |
| `Prefabs/` | 238 | the placeable set |
| `Meshes/` | 213 | FBX sources for the prefabs |
| `Materials/` | 11 | atlas color-sheet materials, grass, mud, water |
| `Texture/` | 6 | atlas, grass and sand/dirt tiling textures |
| `Terrain/` | 7 | one demo `TerrainData` asset and layer settings |
| `Scene/` | 5 | demo scene `Slavica Free.unity`, a 2022 variant, asset preview |

## Categories

| Category | Prefabs | Covers |
| --- | --- | --- |
| [Environment](Environment/README.md) | 41 | roads, ground patches, rocks, bridges |
| [Nature](Nature/README.md) | 34 | trees, bushes, plants, grass |
| [RiverStraight](RiverStraight/README.md) | 4 | straight river pieces, 20 - 59 m |
| [Terrain](Terrain/README.md) | 2 | field tiles |
| [Fence](Fence/README.md) | 16 | fences, walls, gates |
| [Compound](Compound/README.md) | 24 | modular hut parts |
| [Village](Village/README.md) | 17 | finished village buildings |
| [Town](Town/README.md) | 10 | town buildings |
| [Prop](Prop/README.md) | 54 | containers, furniture, scatter |
| [Items](Items/README.md) | 31 | small interior objects |
| [Rooms](Rooms/README.md) | 5 | pre-composed interiors |

## Prefab anatomy

Every prefab with a size has: a root collider (`MeshCollider` on most, `BoxCollider` on some
walls and furniture, none on grass, plants and most bushes); children
`LOD0..LOD2` and a `LOD Group`; one shared atlas material
(`EA03_LPNP-Slavika_Colorsheet_*`), so the kit batches together. `Town/Building`
and `Rooms` have no LOD group.

The pivot is at the prefab origin with scale 1. `LOD Group` `m_Size` is the
largest dimension in meters, approximate; confirm in the Editor before snapping.

Naming: `EA03_<Category>_<Name>_<variant>_PRE`; variant letters are alternates.

## How the demo scene is built

`Scene/Slavica Free.unity` has 11,029 prefab instances of 65 distinct prefabs. It
is a Unity `Terrain` with prefabs on top, not a tile grid.

| Item | Instances |
| --- | --- |
| `Plant_Grass_02a` | 10,389 (94%) |
| `Plant_Grass_01e` | 344 |
| `Nature_Tree_01b` | 107 |
| `Environment_Rock_Mini_Head_01a` | 45 |
| `Fence_Plank_02a..d` | 33 |
| everything else | about 110 |

Root objects: `Terrain`, `Trees`, `Rocks`, `Grass`, `Grass_Spawn` (x4), `Fences`,
`Props`, `Enviro`, `System`, `Post-process Volume`, `Directional Light`,
`Camera`. The ground and elevation come from the terrain; prefabs only stand on it.

## Fit against the sample map's region types

| Region type | Pack support |
| --- | --- |
| forest | strong: Nature trees, bushes, plants, grass |
| meadow / field | strong: grass, plants, `Terrain/Field` |
| village / hub | strong: Village, Town, Fence, Environment roads, Prop |
| rocky / cliff | partial: Environment rocks, stone fences; no cliff-wall module |
| sand / mud | partial: flat ground patches only |
| river | partial: straight segments only |
| deep water / sea | none: needs a custom plane with `Water.mat` |
| cave / dark zone | none |

## Findings that change the plan

1. No prefab fills a region's ground. Ground comes from a Unity `Terrain` with
   the pack's tiling textures, or from custom meshes.
2. Deep water, cliffs and caves have no assets; blockout shapes stand in.
3. Props are sized at 1:1 meters (8 m hut, 5 m road tile), so the pack sets the
   map's world scale.
4. `TODO`: terrain size, resolution and layers are in the binary
   `Terrain/SLAVICA FREE TERRAIN.asset`; read them with an Editor script.
5. Materials used by prefabs are URP Lit; see [AUDIT.md](AUDIT.md) for the full audit.
   `TODO`: confirm they look right in the Editor.
6. `TODO`: the pack's license is not in the repo; check the Asset Store terms
   before shipping anything built from it.

Pipeline context: `docs/map-to-environment/README.md`.
