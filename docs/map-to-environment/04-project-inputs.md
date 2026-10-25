# Project Inputs

Facts that differ per project. Fill every `TODO` before stage 1.

| Input | Value |
| --- | --- |
| Unity version | TODO: from `ProjectSettings/ProjectVersion.txt` |
| Map image | TODO: clean file, annotations removed |
| Legend | TODO: color -> region type -> border type table |
| 3D asset folder | TODO: path under `Assets/` (none imported when this was written) |
| Asset organisation | TODO: modular tile kit, free props, or both |
| Output form | TODO: flat prefab placement or Unity Terrain with height |
| World scale | TODO: world units across the map's longest side |
| Map source and license | TODO: sample is from a CC-BY-SA wiki; confirm for shipped maps |
| Module name and location | TODO: follow the project's module layout |

## Legend template

| Color (hex) | Region type | Ground material | Prop rule set | Border type |
| --- | --- | --- | --- | --- |
| TODO | TODO | TODO | TODO | TODO |

## Open questions

1. Is the map input always a color image, or also heightmap plus masks?
2. Is the sample map a test only, or the shipped layout?
3. Who owns and edits the legend per map?
4. Does gameplay need walkable/blocked data exported with the map?
