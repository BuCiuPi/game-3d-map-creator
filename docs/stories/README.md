# Stories

Stories are work packets. They turn product intent into bounded implementation
and validation work. The lane from `docs/FEATURE_INTAKE.md` decides which packet
a task owes:

| Lane | Packet |
| --- | --- |
| Tiny | None — patch directly |
| Normal | One file from `docs/templates/story.md` |
| High-risk | A folder from `docs/templates/high-risk-story/` |

Stories are written on the branch of the work they describe, and merge with it.

## Normal Story

Use `docs/templates/story.md`.

Suggested path:

```text
docs/stories/<area>/<story-slug>.md
```

## High-Risk Story

Use `docs/templates/high-risk-story/` when the feature intake classifies work as
high-risk.

Suggested path:

```text
docs/stories/<story-slug>/
  overview.md
  design.md
  execplan.md
  validation.md
```

## Proof

Every normal and high-risk story has at least one row in `docs/TEST_MATRIX.md`
whose ID is the story's slug (its file or folder name), so
`docs/HARNESS_AUDIT.md` can match the two. The row's status follows the story.

## Status Flow

```text
planned -> in_progress -> implemented
                  |
                  v
               changed
                  |
                  v
               retired
```
