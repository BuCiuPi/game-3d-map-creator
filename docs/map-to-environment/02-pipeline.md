# Pipeline

Four stages. Each produces data or an asset that the next stage reads; a human
approves at stage 2.

| # | Stage | Input | Output | Approval |
| --- | --- | --- | --- | --- |
| 1 | Inspect | clean map image, legend | region model (data) | automatic, checked in stage 2 |
| 2 | Draft | region model, source image | edited, labeled region model | **human approves** |
| 3 | Blockout | approved draft, scale | greybox scene | human checks proportions |
| 4 | Real map | blockout, 3D assets, rules, seed | baked scene or prefab | human review |

## 1. Inspect

1. Crop to the map; mask annotations.
2. Quantize colors to the legend's palette; nearest-color match with a
   tolerance, so anti-aliased edges snap to a region.
3. Label connected components per color. Drop components under a minimum area
   (noise).
4. Per region: outline polygon, area, centroid, bounding box, neighbors.
5. Per border: shared edge between two regions, so a border type (cliff, wall,
   water edge) can attach to it.

Output is data only. No scene is touched.

## 2. Draft

Show the region model over the source image in the Editor. The user can fix a
mis-segmented region, merge or split regions, assign a type, and set a border
type. Approval freezes the draft; later stages must not edit it.

## 3. Blockout

Greybox at real scale: flat or lightly heightened ground per region, simple
volumes along borders and water, a marker per point of interest. Purpose:
check proportions, traversal, sightlines and region sizes before any art is
placed. Uses no project assets.

## 4. Real map

Per region type, a rule set picks ground material and props. Along borders,
place border pieces by outline. Scatter props by density with a seeded random
generator, so the same draft and seed rebuild the same map. Bake the result.

## Determinism

Same image + legend + draft + seed + asset set must give the same output.
Never use unseeded randomness or the iteration order of unordered collections.

## Non-goals

- No runtime generation: the Editor builds, games ship the baked result.
- No AI/ML segmentation: the legend defines the regions.
