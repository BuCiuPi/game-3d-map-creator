---
paths:
  - "Assets/game.ui/**/*.cs"
  - "Assets/**/UI/**/*.cs"
  - "Assets/**/*.prefab"
---

# Unity UI Rules

## Current Stack And Boundary

The template uses uGUI and TextMesh Pro. Its reusable shell code lives in
`Assets/game.ui/`; it has no package UI base classes, lobby framework, or game
business screens.

The shipped shell contains:

- `LoadingOverlayView` for boot progress and recovery;
- `MessageOverlayView` for a neutral two-action message;
- `ToastView` for transient, non-blocking text; and
- `GlobalUiShellView` as the stable prefab facade.

Keep these components generic. A game-specific popup, HUD, screen flow, store,
quest, or authentication view belongs to a feature in the derived project.

## Placement

- Boot-required overlays:
  `Assets/game.assetbundles/00_local/ui/shell/`.
- Persistent generic shell, message overlay, and toast:
  `Assets/game.assetbundles/01_init/ui/00_shared/`.
- A future feature's UI:
  `Assets/game.assetbundles/02_partial/<feature>/ui/`, created only with that
  feature's first consumer.

Do not put Addressable source prefabs in `Assets/game.contents/`,
`Assets/AddressableAssetsData/`, or a package.

## View Responsibilities

- Serialize Unity references; do not search the scene at runtime.
- Render values and expose user intent.
- Keep business rules, persistence, networking, and long-lived state outside the
  view.
- Expose button intent as an R3 `Observable<Unit>` when a caller needs to
  subscribe.
- Bind subscriptions to an explicit owner (`AddTo(this)` or a feature-owned
  disposable scope).
- A view that starts a coroutine owns and stops it when hidden or destroyed.
- A `CanvasGroup` visibility change must update alpha, interaction, and raycast
  behavior deliberately.

## Serialized Field Naming

Use the current compact prefixes:

- `txt...` for `TMP_Text`;
- `btn...` for `Button`;
- `img...` for `Image`; and
- descriptive nouns for `CanvasGroup`, transforms, child views, and assets.

Fields are `private [SerializeField]`. Expose read-only properties only when a
consumer needs them.

## Prefab Contract

- Keep script and prefab GUIDs stable during moves.
- Wire references through Unity serialization, not YAML text edits.
- Shared prefabs must not reference feature-private assets.
- Feature A must not reference Feature B's private UI.
- An Addressable-instantiated shell is released by the code that owns its
  `AsyncOperationHandle`.

## Proof

For a UI change, run:

1. clean Unity compile;
2. EditMode tests for pure behavior;
3. a PlayMode or editor-driven proof for prefab wiring and interaction;
4. Addressables static check for asset moves; and
5. an Addressables build/diff when membership or dependency edges change.
