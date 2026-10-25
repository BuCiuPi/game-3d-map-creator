# Audit: Slavic World Free

Date: 2026-10-25. Scope: `Assets/EmaceArt/Slavic World Free/` (88 MB), checked
from files on disk (YAML, `.meta`, md5). Not checked: how anything looks in the
Editor; no screenshot was taken.

## Verdict

Usable for the map pipeline as is. Prefabs are clean: every prefab references
only assets inside the pack, and all materials prefabs use are URP Lit. The
problems are in the demo scenes (missing scripts, 52 MB of scene files), in one
unused built-in material, and in import settings that cost memory.

## Findings

| # | Severity | Finding | Evidence | Action |
| --- | --- | --- | --- | --- |
| 1 | Medium | 7 script GUIDs in the 3 demo scenes resolve to nothing: Missing Script components | Unresolved in `Assets/`, `Packages/` and `Library/PackageCache/`. They sit on `Camera`, `Post-process Volume`, plant and grass groups, and the `AssetPreview` group objects | Treat the demo scenes as reference only; never build on them. Do not copy their cameras |
| 2 | Medium | 138 of 213 FBX files have Read/Write enabled | `isReadable: 1` in the `.meta` files | Turn off unless a mesh is read at runtime; it keeps a CPU copy of every mesh |
| 3 | Low | `EA03_LPNP-Slavika_Colorsheet_wind_ForStandardRender.mat` uses `Plugin/Grass.shader`, a built-in surface shader (`#pragma surface ... Standard`) that does not render in URP | shader GUID resolves to `Plugin/Grass.shader` | Unused by any prefab or scene; ignore or park |
| 4 | Low | Both terrain materials use built-in shader `fileID 10623`, not URP Terrain Lit | `Terrain/SlavicaTerrainDemo.mat`, `Terrain/TerrainSetting/SlavicaTerrainFree.mat` | Unused by the scene file; when building a terrain, use URP `TerrainLit` |
| 5 | Low | The wind tree material is URP Lit, so the pack's wind animation is gone | `EA03_LPNP-Slavika_Colorsheet_wind_tree.mat` shader is URP Lit | Accept static trees, or add a wind shader later |
| 6 | Low | Demo scene files are 39.2 MB and 13.1 MB, committed without Git LFS | `Scene/Slavica Free.unity`, `Scene/Slavica Free 2022 Version.unity`; `.gitattributes` has no LFS rule | Fine at this size. If more packs come, decide on LFS or excluding demo scenes |
| 7 | Low | The pack ships its own six URP pipeline assets and a renderer; the project uses `Assets/Settings/PC_RPAsset` and `Mobile_RPAsset` | `Post Processing_profile/Settings/`, `ProjectSettings/QualitySettings.asset` | Unused; do not assign them |
| 8 | Info | `Texture/Flower_Grass.png` has Read/Write on; `Flower_Grass_NM.png` (1.2 MB) is used by no material | texture `.meta`, reference scan | Turn off Read/Write; the normal map can be parked |
| 9 | Info | One mesh, `Meshes/EA03_Plant_Grass_02b.fbx`, is used by no prefab or scene | reference scan | Leave; it costs nothing until referenced |
| 10 | Info | License terms are not in the repo | only `Thanks and checkout this/Info.txt` and a PDF | Check the Asset Store EULA before shipping |

## What passed

- **Materials.** 10 of the 11 files in `Materials/` use URP Lit
  (`933532a4fcc9baf4fa0491de14d08ed7`, `Lit.shader` in URP 17.3.0). The skybox
  uses built-in `Skybox/6 Sided`, which works in URP. 215 prefab and scene files
  use one material: `EA03_LPNP-Slavika_Colorsheet_v1.mat`.
- **References.** No prefab references a GUID outside the pack. All 9 outside
  GUIDs come from the demo scenes: 2 resolve to URP camera and light data, 7 are
  missing (finding 1).
- **Duplicates.** No two files in the pack have the same md5.
- **Scale.** All 213 FBX files import at `globalScale: 1`.
- **Textures.** All 15 are capped at 2048.

## Re-running

The checks came from a one-off script that maps every `.meta` GUID in the pack,
then scans prefabs, scenes and materials for `guid:` references, reads shader
GUIDs, texture and FBX import settings, and hashes files. Recreate it when the
pack changes; it was not kept in the repo.
