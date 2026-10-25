---
name: feature-domain-service
description: "Design a feature-owned service slice in a game derived from the thin Unity template. Use to decide model, gateway, service, state and DI ownership. Works for local-only features and for APIs already activated through Git UPM."
---

# Feature Domain Service

Use this skill when the first real consumer defines a feature boundary. Do not
pre-populate the shared template with speculative business services.

## Slice Shape

```text
feature UI -> feature service/use case -> gateway interface -> local or remote implementation
```

Keep the slice under a feature-owned root chosen by the derived project. Only
stable, game-agnostic primitives may move into `Assets/game.core/`, and that
requires the core public-API hard gate.

## Design Steps

1. Name the user-facing capability and its first consumer.
2. Define the feature model independently from persistence or wire data.
3. Define the smallest gateway interface the service needs.
4. Put orchestration, validation, caching, and state transitions in the service.
5. Choose lifetime from ownership: application/session, scene, screen, or call.
6. Register the implementation in the nearest scope that outlives every
   consumer and no longer.
7. Add feature-focused tests before exposing the service to UI.

## Local Versus Remote

- Local-only features need no networking package.
- A remote feature first follows
  `docs/architecture/networking-on-demand.md`, completes
  `.claude/api-codegen-config.md`, then uses `api-codegen` and
  `gateway-wiring`.
- Do not add networking to the template because a future game might need it.

## Invariants

- `game.core` stays independent from feature, UI, transport, and project
  bootstrap assemblies.
- Wire DTOs and storage records do not escape their gateway.
- UI does not own persistence, transport, or long-lived mutable state.
- Cancellation and disposal follow the owning scope.
- A feature does not reach sideways into another feature's private assets or
  service implementation.

## Proof

Show the target paths and public signatures in the plan, then prove compile,
service behavior, DI resolution, cancellation/disposal, and the relevant
EditMode or PlayMode path. If networking is involved, include the exact pinned
package revision and contract evidence.
