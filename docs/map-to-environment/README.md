# Map-to-Environment

Portable knowledge pack: turn a 2D map image into a playable 3D Unity
environment using 3D assets that already exist in the project.

```text
map image -> inspect -> draft -> blockout -> real map
```

**Status: design only, nothing implemented.** Each file marks what is decided
and what is `TODO`. Update status lines when a stage ships.

## Moving this pack to another project

The folder is self-contained: copy `docs/map-to-environment/` as-is. It links
only to its own files and names no repository-specific path, type, or tool.
After copying, do three things:

1. Fill the `TODO` lines in `04-project-inputs.md` for the new project.
2. Replace the sample map in `01-reference-map.md` or keep it as a worked example.
3. Check the Unity version and module layout in `03-unity-implementation.md`
   against the new project.

## Files

| File | Use |
| --- | --- |
| [01-reference-map.md](01-reference-map.md) | what the first sample map contains and what it cannot tell us |
| [02-pipeline.md](02-pipeline.md) | the four stages: inputs, outputs, approval points |
| [03-unity-implementation.md](03-unity-implementation.md) | proposed module split, data model, testing, baking |
| [04-project-inputs.md](04-project-inputs.md) | per-project facts to fill: assets, legend, scale, open questions |
