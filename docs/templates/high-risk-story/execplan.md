# Exec Plan

## Goal

What outcome are we trying to produce?

## Scope

In scope:

- Item.

Out of scope:

- Item.

## Risk Classification

Risk flags (from `docs/FEATURE_INTAKE.md`):

- Flag.

Hard gates:

- Gate.

## Work Phases

1. Discovery.
2. Design.
3. Validation planning.
4. Implementation.
5. Verification (the proofs the lane requires in `docs/TEST_MATRIX.md`).
6. Harness update (docs/decisions).

## Stop Conditions

Pause for human confirmation if:

- Behavior is ambiguous.
- A contract another component, service, or team depends on would change.
- Production data, money, or user accounts are at risk.
- A validation expectation would be weakened.
- A change would add, remove, or repin a Git UPM capability or restore a retired local package/submodule.
- A change would alter the `game.core` public API.
- An asset move would change `.meta` GUIDs or addresses.
- A new `LifetimeScope` or a changed boot order.
