# 0011 No Zero-Consumer Vendored Assets

Date: 2026-10-25

## Status

Accepted

## Context

The owner chose a thin bootable template that agents compose with project
modules later. The live template has no consumer for 581.03 MiB of inherited
art packs, samples, game utilities, animation/debug/editor plugins, yet every
new game clones them and inherits their import/build surface.

Story: `docs/stories/vendored-asset-prune/overview.md`.

## Decision

The shared template does not vendor an asset, sample, runtime plugin, or editor
tool without a live template consumer. Optional capabilities are installed by a
derived project with their first real consumer and are pinned or versioned by
that project. Tool source and samples are not kept merely as examples.

Package-manifest dependencies are evaluated in a separate cohort so their lock
and transitive graph receive independent proof.

## Consequences

Positive:

- Normal clones and full imports are substantially smaller.
- New games begin without unrelated paid/tool-specific settings and defines.
- Re-adding a capability is an explicit, reviewable project decision.

Tradeoffs:

- A game that needs DOTween, Odin, Spine, Feel, SRDebugger, or another removed
  tool must install and configure it.
- Historical samples are recovered from Git history or the provider, not from
  the template.

## Follow-Up

- Audit direct UPM packages and NuGet payloads by live consumer.
- Keep TextMesh Pro resources and current live packages until that audit proves
  an independent removal cohort.
