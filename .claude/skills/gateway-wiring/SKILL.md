---
name: gateway-wiring
description: "Wire a configured networking request through a feature-owned gateway, service and VContainer scope in a derived game. Use after api-codegen has a real contract. Not for selecting or installing a networking package."
---

# Gateway Wiring

The template contains no live gateway stack. This skill activates only in a
derived project whose `.claude/api-codegen-config.md` is `configured` and whose
networking package already resolves and compiles.

## Boundaries

```text
UI/presenter -> feature service -> feature gateway -> mounted transport
```

- UI depends on a service or use-case interface.
- Domain code does not know request/response types.
- The gateway owns transport calls, wire-to-model mapping, and transport-error
  normalization.
- The composition root chooses the concrete gateway.

## Lifetime Choice

- Application/session-wide state: register once in the derived project's root
  scope.
- Scene-owned state: register in that scene's scope.
- Stateless mapping and small services may be transient when construction is
  cheap.
- Never re-register a parent-owned stateful service in a child scope.

The neutral template has no `LobbyLifeTimeScope`; the derived project must name
its actual composition root in `.claude/api-codegen-config.md`.

## Required Behavior

- Return the configured result/error abstraction; do not leak transport
  exceptions to presenters.
- Treat non-2xx, server-declared failure, malformed/empty payload, timeout,
  cancellation, and offline behavior explicitly.
- Keep auth/token acquisition behind the configured seam.
- Propagate `CancellationToken`.
- Do not log secrets or raw auth material.
- Register one concrete gateway instance under all interfaces it implements.

## Proof

At minimum, prove:

- clean Unity compile;
- request serialization and mapping tests;
- success plus each supported failure shape;
- cancellation;
- DI resolution from the intended scope; and
- no dependency from `game.core` to the feature/networking assembly.

If the package or config is absent, stop and use
`docs/architecture/networking-on-demand.md`; never restore a local `file:`
dependency or a retired repository submodule.
