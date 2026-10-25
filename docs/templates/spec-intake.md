# Initiative / Spec Intake

Use for a large feature or a new product area — work too big for one story.
Scope it here first, then slice into stories. Do not create every story packet
up front.

Date: YYYY-MM-DD

## Source

Where did this come from? (product ask, design doc, mockup, external reference)

## Summary

What are we building, for which part of the product, and why?

## Affected Areas

Which parts of the codebase this touches. Reference the knowledge base.

| Area | Path | Notes |
| --- | --- | --- |
| | `path/...` | |

## Candidate Stories

Only the slices clear enough to name.

| Story | Description | Lane | Status |
| --- | --- | --- | --- |
| US-XXX | | | unsliced |

## Architecture Questions

- Does this belong in the template, or in a feature module a project adds?
- Which assembly owns it, and does it keep core/UI/bootstrap/feature boundaries one-way?
- Does it need a composition root or scope in the derived project?
- Which Addressables tier does each new asset go in (`00_local`, `01_init`, `02_partial`)?
- Does it add or change a direct Git UPM capability?
- Does this introduce or change a contract another component depends on?

## Validation Shape

What proof will this eventually need? (see `docs/TEST_MATRIX.md`)

## Open Decisions

- Item — record accepted ones under `docs/decisions/`.

## Harness Delta

Knowledge/spec docs or decisions to create or update because of this initiative.
