# Unity UI Architecture Patterns — MVP, uGUI, UI Toolkit

Type: Reference (stack-level). Patterns common to Unity clients, not facts about
this repository. Which stack and which variant the live code uses is recorded in
the numbered knowledge files and in `.claude/rules/ui-code.md`.

## Choosing the stack per screen

| | uGUI | UI Toolkit (UITK) |
| --- | --- | --- |
| Layout | Prefab hierarchy, `RectTransform`, layout groups | UXML + USS flexbox |
| Styling | Inspector properties, prefab variants | USS classes and custom properties (tokens) |
| Element binding | `[SerializeField]` references | `Q<T>("name")` queries, or runtime data binding (Unity 6+) |
| Repeating rows | Nested prefabs, pooled cells | Row templates (`VisualTreeAsset`) cloned at runtime |
| World-space / effects | Mature (particles, masks, world canvases) | Limited; check the Unity version |
| Text | TextMeshPro | Built-in text with font assets |

A project can run both. Review a screen against the stack it actually uses; the
signal is in the file (`Canvas`/`TMP_Text`/prefab vs `UIDocument`/`.uxml`).

## MVP roles (both stacks)

- **Presenter** (or Controller): state, decisions, flow, subscriptions to View
  inputs and to domain events; calls services. Never touches rendering
  primitives directly.
- **View**: serialized references or cached elements, input observables
  (`Observable<Unit>` per interaction), setters (`SetCount`, `Show`, `Hide`),
  animation and tween cleanup. No business logic.
- **Model / data**: POCO or record; no MonoBehaviour.
- Direction: Presenter → View by method; View → Presenter by observable only.

### uGUI variant

`<Feature>Presenter.cs` + `<Feature>View.cs`, both MonoBehaviours, wired by
`[SerializeField]`. No View interface is required; the prefab is the contract.
Base presenters/views shared across games or modes are extended by override,
calling the base binding first.

### UITK variant

`<Screen>Controller.cs` (presenter role) + `<Screen>View.cs` (MonoBehaviour with
a `UIDocument`) + `I<Screen>View.cs`. The controller holds the interface, which
keeps it ignorant of element queries and lets a screen swap stacks. Shared
sub-templates (header, footer, a row) get one wrapper object each that queries
and caches its elements once and exposes typed members; the View never
re-queries a shared subtree.

## Canvas and scaling (uGUI)

- A `CanvasScaler` with *Scale With Screen Size* and a reference resolution
  equal to the design artboard makes one design pixel one UI unit.
- Separate canvases for content that changes at different rates (a ticking
  timer on the same canvas as a static panel rebuilds the whole canvas).
- Sort order decides overlap between canvases; modals sit above the shared
  header/footer canvas. Two canvases at the same order race.
- Orientation variants (portrait/landscape, mobile/PC) are usually separate
  canvases or prefab variants switched at runtime.

## Responsive math (UITK)

A common rule set when converting a fixed design artboard to UITK:

```text
width:     (design_px / artboard_width * 100)%
min-width: round(design_px * 0.75)px
max-width: round(design_px * 1.25)px
height:    design_px px   (fixed until a responsive pass)
```

Read the artboard width from the design frame each time; never assume one.
Absolute positioning only for intentional overlaps.

## Runtime data binding (Unity 6+ UITK)

- Data source properties carry `[CreateProperty]`; the backing field carries
  `[SerializeField, DontCreateProperty]` — never both on one member.
- Declare static bindings in UXML with `data-source-path` and set
  `element.dataSource = model` at runtime.
- Sprites and class toggles stay in C# (`EnableInClassList`); prefer
  `BindingMode.ToTarget` for display-only labels.

## Lists and pooling

- Pool cells for any list that scrolls or refreshes; the DI container does not
  see pooled instances, so data goes in by method and events come out by
  observable.
- A pooled cell resets all visual state on reuse; a stale tween or toggled
  child is the classic pooled-list bug.

## Navigation and modals

- One navigation owner (a page stack / modal container service) decides what is
  shown; views do not open each other directly.
- A modal shown from a global layer resolves its dependencies from the scope it
  is built from — usually the root — so it cannot see scene-scoped services.
- Releasing a page releases its child DI scope before the parent disposes.

## Figma (or other design tool) to Unity

- Layer naming is the contract between art and code: a `tag - name` layer
  convention (`btn`, `img`, `txt`, `cont`, `scroll`) maps to element names and
  serialized field names. Button states (`def`, `pres`, `hov`, `dis`) are
  children of the button, never siblings.
- Correct names in the implementation even when the design file is not yet
  fixed, and report the design-side fixes.
- Reuse before building: an existing shared component, then a modifier of one,
  then a promotion candidate (used by two screens), then inline.
