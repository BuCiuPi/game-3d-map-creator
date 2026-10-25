---
paths:
  - "**/*.cs"
---

# C# Code Style

## Scope

These rules apply to code owned by the repository. Do not restyle vendored code
under `Assets/Plugins/`, `Assets/Spine/`, `Assets/AlmostEngine/`,
`Assets/StompyRobot/`, `Assets/Firebase/`, or `Assets/UnityAssetStore/`
unless the task explicitly owns that dependency.

## Layout

- One primary public type per file; file name matches the type.
- Use the file-scoped convention already present in the target assembly. Current
  template code uses a block namespace with `using` directives inside it.
- Order members so configuration/serialized fields come first, then state,
  accessors, Unity lifecycle, public methods, and private helpers.
- Regions are optional. Preserve useful regions in an existing file; do not add
  empty or one-member regions.
- Keep methods small enough that state ownership and failure paths are visible.

## Naming

- `PascalCase`: types, methods, properties, constants.
- `camelCase`: parameters, locals, private fields.
- Interfaces start with `I`.
- Async methods end in `Async`.
- Names describe the capability, not the current implementation.
- Avoid abbreviations except established Unity/project terms such as UI, DT, DA,
  GUID, API, and UPM.

## Unity Serialization

- Use `private [SerializeField]` for inspector wiring.
- Do not make a field public only to serialize it.
- Renaming a serialized field requires `[FormerlySerializedAs]` or an explicit
  migration.
- Never hand-edit managed-reference YAML.
- Preserve `.meta` files and GUIDs when moving or renaming assets/scripts.

## Null And Failure Behavior

- Validate required serialized dependencies at the owning boundary.
- Do not swallow exceptions. Convert them only where the caller has a meaningful
  error abstraction or recovery path.
- Keep user-facing recovery neutral; log technical detail without secrets.
- Avoid null-forgiving operators unless an invariant is proven in the same type.

## Async, Coroutines, And Disposal

- Thread `CancellationToken` through async feature boundaries.
- Tie work to the lifetime that owns it.
- Independent async operations may run together; order-dependent operations stay
  sequential.
- Fire-and-forget must surface exceptions through the project's chosen helper or
  an explicit handler.
- R3 subscriptions and Addressables handles have an explicit owner and release
  path.
- A coroutine started by a view is stopped or made harmless when the view is
  hidden/destroyed.

## Logging

Use `Debug.unityLogger` or an injected abstraction when logging is required.
Never log tokens, credentials, authorization headers, or full sensitive
payloads. Avoid expensive interpolation when logging is disabled.

## Comments

Comments explain intent, ownership, invariants, or non-obvious constraints. Do
not narrate the syntax. Public XML documentation is required only when the
contract would otherwise be ambiguous.

## Template Boundary

- `game.core` cannot depend on UI, bootstrap, feature, transport, or internal
  package assemblies.
- The template has no `com.paul.*`/ `Paul.*` runtime contract.
- Optional capabilities are mounted in a derived project through pinned Git UPM
  only when a real consumer exists.
