# Validation

## Proof Strategy

What must pass before the story is done (see `docs/TEST_MATRIX.md`).

## Test Plan

| Proof | Cases |
| --- | --- |
| `compile` | every C# change |
| `editmode` | `Assets/game.core/` data primitives and any plain C# logic |
| `boot` | bootstrap, lifetime scope, Addressables init, scene routing |
| `editor-evidence` | UI screens, popups, prefab wiring, Addressables menu passes |
| `addressables-build` | group, profile, layout-rule or asset-move changes |
| `platform` | a device build of a game made from the template |

## Fixtures

Deterministic accounts, mock services, seed data, or test environments needed
for repeatable proof.

## Acceptance Evidence

Add screenshots, test output, and links after verification.
