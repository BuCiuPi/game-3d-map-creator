# Asset Pack: Slavic World Free (EmaceArt)

First asset set for the pipeline. Read it as a worked example of the facts a
stage-4 rule set needs about any pack. Facts below came from the prefab and
scene YAML under `Assets/EmaceArt/Slavic World Free/` on 2026-10-25.

## What it is

A low-poly Slavic village/forest kit. Free sample of a larger paid pack (see
`Thanks and checkout this/Info.txt`). Built for Unity 2019 Standard render;
URP support is a separate folder (`URP_Support/`). The project is on URP 17, so
check that materials render before relying on them.

| Folder | Count (non-.meta) | Use |
| --- | --- | --- |
| `Prefabs/` | 238 | the placeable set |
| `Meshes/` | 213 | FBX sources for the prefabs |
| `Materials/` | 11 | color-sheet atlas materials, grass, mud, water |
| `Texture/` | 6 | atlas, grass and sand/dirt tiling textures |
| `Terrain/` | 7 | one demo `TerrainData` asset and layer settings |
| `Scene/` | 5 | demo scene `Slavica Free.unity`, a 2022 variant, asset preview |

## Prefab anatomy (all prefabs share it)

Every prefab, e.g. `EA03_Environment_Road_Cobble_01a_PRE`, has:

- a root with a collider: `MeshCollider` on most, `BoxCollider` on some walls
  and the foundation, none on grass/plants/bushes;
- children `LOD0`, `LOD1`, `LOD2` plus a `LOD Group`;
- one shared material, the color-sheet atlas (`EA03_LPNP-Slavika_Colorsheet_*`),
  so the whole kit batches together.

Pivot is at the prefab origin (`m_LocalPosition` 0,0,0, scale 1,1,1). The
`LOD Group` `m_Size` is the object's approximate largest dimension in meters.
It is not exact bounds. Use it to pick a grid size, then confirm in the Editor.

Naming: `EA03_<Category>_<Name>_<variant>_PRE`. Variant letters (`01a`, `01b`)
are alternates of one shape.

## Categories and footprints

Sizes are LOD Group `m_Size` ranges in meters.

| Group | Prefabs | Size (m) | Role in a map |
| --- | --- | --- | --- |
| `Environment/Road` | 5 | 1.9 - 5.4 | cobble road tiles (5.2 m main tile), corner, wooden road, platform |
| `Environment/RoadIsland` | 5 | 3.9 - 18.7 | irregular road patches |
| `Environment/Sand`, `Mud` | 6, 5 | 3.9 - 19.6 | flat ground patches (ground decals/slabs) |
| `Environment/Rock` | 12 | 0.6 - 19.6 | rocks, from small heads to large formations |
| `Environment/Bridge`, `Stairs` | 2, 1 | 4.6 - 6.6 | piers, steps |
| `RiverStraight` | 4 | 20.5 - 58.7 | straight river segments (four lengths) |
| `Terrain/Field` | 2 | 20.0 | 20 m field tile with a seam variant |
| `Nature/Tree` | 7 | 9.3 - 16.8 | trees; mostly `MeshCollider` |
| `Nature/Bushes`, `Plant`, `Grass` | 14, 9, 4 | 0.5 - 5.9 | scatter vegetation; no collider |
| `Fence/*` | 16 | 1.2 - 6.3 | plank, wall, cobble, gate; plank fence is 4 m |
| `Compound/*` | 24 | 4.7 - 20.1 | modular hut parts: foundation, walls, roofs, porch, balcony |
| `Village/*`, `Town/*` | 27 | 0.7 - 24.5 | finished buildings, sheds, outbuildings |
| `Prop/*`, `Items/*` | 85 | under 3 (largest in scene: bed 2.8) | containers, furniture, crockery, tools |

Rough grid guide: roads about 5 m, fences 4 m, field and foundation tiles 20 m.
Rivers are long single pieces (20 - 59 m), not tiles. There is no sea or
deep-water prefab; `Materials/Water.mat` exists for custom planes.

Mapping to the sample map's region types:

| Region type | Pack support |
| --- | --- |
| forest | strong: 7 trees, 14 bushes, 9 plants, grass |
| meadow / field | strong: grass, `Terrain/Field`, plants |
| village / hub | strong: huts, walls, fences, roads, props |
| rocky / cliff | partial: 12 rocks, rock fences; no cliff-wall modules |
| sand / mud | partial: flat ground patches only |
| river | partial: straight segments only, no bends or banks |
| deep water / sea | none: needs a custom plane with `Water.mat` |
| cave / dark zone | none |

## How the demo scene is built

`Scene/Slavica Free.unity` has 11,029 prefab instances of only 65 distinct
prefabs. It is a Unity `Terrain` with prefabs scattered on top, not a tile grid.

| Item | Instances | Note |
| --- | --- | --- |
| `EA03_Plant_Grass_02a` | 10,389 | 94% of the scene; small grass tufts |
| `EA03_Plant_Grass_01e` | 344 | larger grass |
| `EA03_Nature_Tree_01b` | 107 | the one forest tree |
| `EA03_Environment_Rock_Mini_Head_01a` | 45 | small rocks |
| `EA03_Fence_Plank_02a..d` | 33 | plank fences |
| Everything else | about 110 | props, huts, road tiles |

Root objects: `Terrain`, `Trees`, `Rocks`, `Grass`, `Grass_Spawn` (x4),
`Fences`, `Props`, `Enviro`, `System`, `Post-process Volume`,
`Directional Light`, `Camera`.

Takeaway for the pipeline: the demo uses the terrain for ground and
elevation, and prefabs only for things standing on it. That fits the
"Unity Terrain plus props" output form in `03-unity-implementation.md`.
Grass must be instanced or painted as terrain detail, since 10 k game objects
is not a sensible target.

## Findings that change the plan

1. The kit has no ground tiles to cover a whole region. Region ground must
   come from a `Terrain` with the pack's tiling textures (`Texture/`), or from
   custom meshes. Prefabs alone cannot fill a map.
2. Regions in the sample map that need deep water, cliffs or caves have no
   assets. Either draw those with blockout shapes or add assets later.
3. Props are sized in meters at 1:1, so world scale is decided by the pack:
   an 8 m hut next to a 5 m road tile. Pick the map's world scale with these in mind.
4. `TODO`: terrain size, resolution and layer list. They sit in the binary asset
   `Terrain/SLAVICA FREE TERRAIN.asset` and can be read with a one-off Editor
   script (`TerrainData.size`, `terrainLayers`).
5. `TODO`: confirm the atlas materials render under URP 17 in the Editor, using
   `URP_Support/URP Info.txt` if they do not.
6. Licensing: the pack's license is not in the repo. `TODO`: check the Asset
   Store terms before shipping anything built from it.

## Re-deriving these numbers

Counts and sizes came from a short script that walks `Prefabs/**/*.prefab`,
reads each `.meta` GUID, and counts `m_SourcePrefab` GUIDs in the scene YAML.
Re-run something equivalent after the pack changes; do not edit the tables by hand.
