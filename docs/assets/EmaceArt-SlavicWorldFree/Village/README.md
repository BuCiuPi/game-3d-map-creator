# Village

Finished village buildings and attachments. 17 prefabs. Table: [prefabs.md](prefabs.md).

| Subgroup | Count | Size (m) | What it is |
| --- | --- | --- | --- |
| `Building/Other` | 5 | 4.8 - 10.9 | outbuildings, a tower, a cubby (`Cubby_01d` is in the scene twice) |
| `Building/Shed` | 1 | 24.5 | the largest building here |
| `Building/Porch` | 1 | 6.6 | porch module |
| `Building/Compound/Walls` | 1 | 5.4 | one hut wall |
| `OutBuilding` | 3 | 5.2 - 11.1 | penthouse, shed, wood-roof sheds |
| `Elements` | 5 | 0.7 - 3.0 | shutters, bolt and similar attachments |
| `Village/AnimalBreeding` | 1 | 2.4 | animal hovel |

## Using it in a map

- These are the place-and-go buildings for village regions. A building is one
  prefab with a mesh collider and LODs.
- Sizes run 5 - 25 m, which sets the scale of a village region: a 24.5 m shed
  next to 5 m road tiles.
- `Elements` are decoration for other buildings, not standalone.

## Gaps

- `TODO`: building entrances and facing are undocumented. Check pivot and front
  direction in the Editor before auto-placing along a road.
