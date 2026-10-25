# Overview

## Current Behavior

The live template code and five Addressable shell prefabs use none of the
vendored art packs, game/editor utilities, animation runtimes, debug consoles,
or inspector/tween packages stored under eight legacy asset roots. Those roots
still add 9,147 tracked files and 581.03 MiB to every clone.

## Target Behavior

The shared template contains only assets and tools with a live template
consumer. A derived game installs an optional editor/runtime capability with its
first real consumer and owns its samples, settings, defines, and upgrade path.

## Scope

- Remove the eight zero-consumer vendored roots and their root `.meta` files.
- Remove DOTween/Odin scripting symbols with their owning plugins.
- Update the dependency inventory, template docs, and boundary invariant.
- Keep UPM/package-manifest cleanup as a separately proven follow-up cohort.

## Non-Goals

- Removing TextMesh Pro resources, R3, Addressables, SmartAddresser, URP, or the
  game-owned bootstrap/UI/data assets.
- Choosing which currently unused UPM packages belong in the long-term base.
- Importing replacements or sample content.
