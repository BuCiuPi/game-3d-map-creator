# Map-to-Environment Pipeline

The planned workflow (map image -> inspect -> draft -> blockout -> real map) is
documented as a portable pack in `docs/map-to-environment/`. Start at its
`README.md`. This file only records how the pack relates to this repository.

Status: **design only, nothing built.** `Assets/` holds no FBX/OBJ/GLB/glTF
files as of 2026-10-25, so the real-map stage has no assets yet.

## Repo constraints on the build

- Feature code goes in its own `Assets/game.<name>/` module with `Runtime`,
  `Editor` and `Tests` assemblies; the template carries no placeholder modules
  (`CLAUDE.md`, decision `0011`).
- Config uses `DataAsset<TData>` / `DataTable<TRow>`
  (`docs/architecture/template-core-data.md`).
- Adding the module is a template-boundary change: high-risk lane, story packet
  and decision record (`docs/FEATURE_INTAKE.md`).
- No package is added without a first consumer (decision `0012`).
