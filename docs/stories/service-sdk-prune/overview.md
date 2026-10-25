# Overview

## Current Behavior

The neutral runtime has no Firebase, ads, attribution, IAP, or Unity Services
consumer, but the repository still carries about 167 MiB of Firebase, EDM4U,
generated Maven artifacts, native mobile libraries, ad-oriented Android build
templates, and unused IAP/cloud packages.

## Target Behavior

The shared template contains no service SDK implementation, generated resolver
repository, product mobile build template, or service package. A derived project
adds one pinned capability only when a real feature consumes it.

## Affected Surfaces

- Vendored service assets and native plugins under `Assets/`.
- Android custom manifest, Gradle, settings, properties, and proguard files.
- `Packages/manifest.json`, `Packages/packages-lock.json`, and scoped registries.
- Project resolver cache and template boundary tests.

## Affected Docs

- `docs/decisions/0010-no-bundled-service-sdks.md`
- `.claude/knowledge/01-project-brief.md`
- `.claude/knowledge/03-build-and-profiles.md`

## Non-Goals

- Pruning art/editor capabilities such as Odin, DOTween, Spine, Feel, or Coffee UI.
- Selecting a target platform or adding signing/build automation.
- Adding networking, auth, analytics, ads, store, or backend replacements.
