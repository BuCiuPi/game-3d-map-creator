---
name: api-codegen
description: "Generate a derived game's request, response, wire-data, model, mapper and tests after its networking package and API contract have been explicitly configured. Use for a real endpoint, OpenAPI operation, or captured payload. Never activates a package or invents a transport contract."
---

# API Codegen

This is a capability skill for games created from the template. The template
itself has no networking package and no API surface.

## Hard Gate

Read `.claude/api-codegen-config.md` first. Continue only when:

- `status = configured`;
- a direct, commit-pinned Git UPM dependency exists in
  `Packages/manifest.json`;
- Unity has resolved it and compilation is clean;
- the receiving feature assembly references the recorded runtime assembly; and
- a real contract source exists.

Otherwise stop and follow `docs/architecture/networking-on-demand.md`. Do not
copy Cricket requests, historical `com.paul.*` examples, or inactive files in
`References/` into the project.

## Contract Priority

1. Captured request/response samples from the target environment.
2. Current OpenAPI/Swagger for the same endpoint.
3. An explicit endpoint description supplied by the owner.

When sources disagree, report the mismatch. Never silently guess the type of a
field that is always `null`, changes shape, or is absent from the contract.

## Generated Slice

Generate only what the configured package and endpoint require:

- request and response wire types;
- wire-data types with explicit serialization names;
- feature-owned model types;
- explicit wire-to-model mapping;
- a feature-owned gateway interface and implementation; and
- EditMode tests for route, verb, serialization, mapping, failure, and
  cancellation behavior.

Keep transport types inside the gateway boundary. UI and domain services receive
models or result values, never raw response envelopes.

## Workflow

1. Inspect the mounted package source/API for the pinned revision.
2. Read the completed config and contract source.
3. Write a small generation plan showing exact target paths and type signatures.
4. Generate the narrowest endpoint slice.
5. Wire it with `gateway-wiring`.
6. Run compile and the new EditMode tests.
7. Record the package revision and contract source in the validation handoff.

## Safety Rules

- Do not place API code in `Assets/game.core/`.
- Do not add a package merely because this skill was invoked.
- Do not log tokens, credentials, raw authorization headers, or full sensitive
  payloads.
- Thread `CancellationToken` through every async boundary.
- Do not claim an endpoint works from serialization tests alone; a live or
  mocked transport proof must be named explicitly.
