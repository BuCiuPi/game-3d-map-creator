# 0010 No Bundled Service SDKs

Date: 2026-10-25

## Status

Accepted

## Context

The owner confirmed that API/service capability is added only when a project has
a related consumer, with Git UPM preferred for team-owned modules. The neutral
runtime has no Firebase, ads, attribution, IAP, or Unity cloud-service consumer,
yet their binaries and build integration were still inherited by every clone.

Story: `docs/stories/service-sdk-prune/overview.md`.

## Decision

The shared template bundles no service SDK implementation, native mobile
integration, generated resolver repository, or service-specific Android build
template. A derived project adds the smallest accepted provider capability with
its first consumer and owns its configuration there.

This does not classify art/editor tooling as a service SDK; those dependencies
need their own consumer and value review before removal.

## Alternatives Considered

1. Keep integrations disabled behind scripting symbols.
2. Keep one Firebase/ads setup as an example project.
3. Remove all third-party tooling in the same commit.

## Consequences

Positive:

- New games do not inherit another product's native service binaries or Gradle graph.
- Package resolution and platform configuration start provider-neutral.
- Re-adding a service produces an explicit, reviewable dependency delta.

Tradeoffs:

- The first service-backed feature must install and configure its provider.
- Platform compilation of a future integration cannot rely on this template as a sample.

## Follow-Up

- Vendored-tool audit completed by decision `0011`; direct UPM/NuGet package audit remains.
- Add platform build proof only after the owner selects a target.
