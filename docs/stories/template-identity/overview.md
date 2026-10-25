# Overview

## Current Behavior

The repository still serializes the Jigsaw company, product, application IDs,
version counters, scripting defines, Android build profiles, and Firebase app
configuration. A game cloned from the template can therefore inherit another
product's identity before it makes an explicit platform choice.

## Target Behavior

The template identifies itself as `Amihan / Unity Template`, uses
`com.amihan.unitytemplate` only as a non-production baseline, starts at version
`0.1.0` / build `1`, and carries no product SDK/profile scripting defines.
Identity is reproducible through an idempotent Editor command. No platform, SDK, remote
catalog, or deployment environment is selected.

## Affected Surfaces

- Project/player settings and scripting defines.
- Legacy Android build-profile assets.
- Product-specific Firebase app configuration and the resolver cache bundle ID.
- Bootstrap Editor tooling and EditMode boundary tests.

## Affected Docs

- `.claude/knowledge/01-project-brief.md`
- `.claude/knowledge/03-build-and-profiles.md`
- `docs/decisions/0009-neutral-template-identity.md`
- `docs/TEST_MATRIX.md`

## Non-Goals

- Selecting Android, iOS, WebGL, or desktop as the product target.
- Removing Firebase, ads, AppsFlyer, AppLovin, or other SDK roots.
- Adding deployment credentials, remote Addressables paths, or build automation.
- Removing capability symbols automatically maintained by retained plugins; C8
  removes those with their unused SDK roots.
