# 0009 Neutral Template Identity

Date: 2026-10-25

## Status

Accepted

## Context

The thin template still inherited Jigsaw player identity, Android profile
overrides, custom define symbols, and Firebase app configuration. Keeping them
would make every derived game start from another shipped product's platform and
service assumptions.

Story: `docs/stories/template-identity/overview.md`.

## Decision

The repository uses `Amihan / Unity Template`,
`com.amihan.unitytemplate`, version `0.1.0`, and build number `1` as explicitly
non-production defaults. The base template has no product SDK/profile scripting
defines and no saved platform build profile. A derived project replaces identity
and adds such a define or optional SDK only when a real feature consumes it.
Capability symbols automatically maintained by retained plugins are removed with
those plugins during C8, not forged away while their source is still present.

The baseline is applied through a permanent Unity Editor command rather than by
editing Unity YAML. Product-specific service configuration is never retained as
sample data.

## Alternatives Considered

1. Keep the Jigsaw settings and require each project to remember to replace them.
2. Use `DefaultCompany` and an automatically generated identifier.
3. Choose Android and preserve its build profile as the template default.

## Consequences

Positive:

- A fresh clone cannot accidentally build with Jigsaw identity or Firebase app configuration.
- The baseline is repeatable in batch mode and mechanically checked.
- Platform and optional SDK choices remain demand-driven.

Tradeoffs:

- Derived projects must intentionally replace the placeholder identity.
- Retained SDK roots remain repository weight until the separate C8 prune.

## Follow-Up

- C8 inventories and removes unused third-party SDK roots, resolver dependencies,
  and generated integration configuration with a fresh compile baseline.
