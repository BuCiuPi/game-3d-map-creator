# 0012 Minimal Live Package Baseline

Date: 2026-10-25

## Status

Accepted

## Context

After service SDK and vendored asset pruning, the manifest still declares 24
optional packages with no live template code or serialized asset consumer. The
committed NuGet payload also includes four unused libraries. Every inherited
dependency expands fresh import time, editor side effects, upgrade work, and the
surface a derived game must understand.

Story: `docs/stories/package-baseline-prune/overview.md`.

## Decision

The template directly declares a non-module package only when it has a current
template consumer or manages a required live dependency. The accepted baseline
is R3, NuGetForUnity, Addressables, URP, Test Framework, uGUI, and
SmartAddresser. Optional packages are installed by a derived project together
with their first real consumer.

The R3 NuGet payload is reduced to R3, Microsoft.Bcl.AsyncInterfaces,
Microsoft.Bcl.TimeProvider, and System.Threading.Channels. NuGetForUnity remains
because it owns that dependency set's update/recovery workflow; changing the
distribution mechanism requires a separate migration with fresh-clone proof.

Explicit built-in `com.unity.modules.*` declarations are not part of this
decision and receive their own audit.

## Consequences

Positive:

- New projects resolve and import a materially smaller package graph.
- Optional features no longer silently dictate settings or editor integrations.
- The remaining baseline is explainable from a live source, asset, test, or
  dependency-management consumer.

Tradeoffs:

- A project needing Input System, Cinemachine, Timeline, Visual Scripting,
  UniTask, VContainer, Coffee UI effects, or other removed capabilities must pin
  and configure them explicitly.
- R3 still uses committed NuGet binaries and the NuGetForUnity editor package.

## Follow-Up

- Prove the reduced package graph in Unity and a normal fresh clone.
- Audit explicit built-in Unity modules independently.
- Migrate HyperEel common core through a pinned Git UPM package only when its
  first retained template capability requires it.
