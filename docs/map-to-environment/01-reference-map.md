# Reference Map

First sample: a screenshot of a community wiki's interactive world map.
Treat it as a test input, not a shipped layout.

## What it contains

- One island-like landmass on a white background with a black outline,
  about 400 x 340 px usable.
- About 12 flat-colored regions with irregular, organic borders.

| Color | Position | Likely meaning (unconfirmed) |
| --- | --- | --- |
| Brown / maroon | north-west | dark zone, possibly cave or hostile |
| Orange, tan | west | dry, desert-like zone |
| Purple | centre | hub or special area |
| Green, teal-green | north-east | forest |
| Light blue, cyan | inside the north-east green | water |
| Blue, dark navy | south-east | deep water or coastal zone |
| Lime green, pale yellow edge | south | meadow with sandy rim |

- Grey rubble rims mark some borders: likely cliffs or walls, not open ground.

## What it cannot tell us

| Missing | Consequence |
| --- | --- |
| Elevation | height must come from rules, a second image, or hand authoring |
| Paths and roads | a path layer must be drawn or generated |
| Scale | world size is a project decision (`04-project-inputs.md`) |
| Legend | the color -> region-type table must be supplied |

## Image hygiene

Pink pins and the red hand-drawn circle in the screenshot are annotations,
not map data. Re-export a clean image, or mask those pixels before inspection.
UI chrome (buttons, dark frame) is also outside the map and must be cropped.

## Licensing

Content on the source wiki is CC-BY-SA. Anything derived from this map and
shipped needs attribution and the same license terms. Check before shipping.
