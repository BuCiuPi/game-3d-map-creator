# Unity Implementation (proposed)

Nothing here exists yet. Names are proposals.

## Module split

One feature module with three assemblies so logic is testable without the Editor.

| Assembly | Holds | References |
| --- | --- | --- |
| `<module>.runtime` | region model, legend, segmentation, placement rules, seeded RNG; plain C# on pixel arrays and structs | Unity core only |
| `<module>.editor` | draft window, blockout builder, baker, menu entries; `UnityEditor` only | runtime |
| `<module>.tests.editmode` | segmentation, determinism, rule tests | runtime, editor |

Runtime must not use `UnityEditor`. Editor depends on runtime, never the reverse.

## Data model

| Type | Fields |
| --- | --- |
| `MapLegend` (ScriptableObject) | entries of color, region-type id, tolerance |
| `RegionType` | id, ground material, prop rule set, default height, walkable |
| `BorderType` | id, piece prefab, spacing, blocks movement |
| `MapDraft` (ScriptableObject) | source texture ref, scale, regions (id, type, polygon, area, neighbors), borders |
| `PlacementRuleSet` | prefab, density, min spacing, slope limit, region filter |

Keep serialized data separate from algorithms, so tests build a `MapDraft` in
memory.

## Algorithms

- Color quantize: nearest legend color in linear RGB, reject beyond tolerance.
- Connected components: flood fill on the label grid.
- Outline: boundary tracing, then Douglas-Peucker simplification to keep
  polygons small.
- Scatter: Poisson-disk sampling inside a region polygon, seeded.
- Height (optional): per-region base height plus smoothed noise, blended across
  borders. Decide before stage 3 (`04-project-inputs.md`).

## Output form (decision needed)

| Option | Fits when |
| --- | --- |
| Flat ground meshes plus prefab placement | modular kit, no elevation |
| Unity `Terrain` from a generated heightmap, props on top | elevation matters |

## Testing

| Check | Proof |
| --- | --- |
| Segmentation | a small synthetic image gives the expected regions and neighbors |
| Determinism | same inputs and seed give identical placements twice |
| Rules | no prop outside its region, spacing respected |
| Bake | an EditMode test builds a scene from a fixture draft without errors |

## Dependencies

Prefer built-in Unity APIs. Add a package only when a stage has a real
consumer for it.
