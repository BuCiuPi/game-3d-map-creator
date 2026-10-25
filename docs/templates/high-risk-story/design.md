# Design

## Data / Models

Entities, data shapes, persisted state, and the rules over them.

## Flow And Wiring

Describe the actual flow for this change: `TemplateBootstrap` and the neutral UI shell, or the derived project's named composition root → feature services/gateways → presenters/views. Do not invent a scope for the shared template.

## Contract Surface

Messages, events, API request/response shapes, and errors — anything a caller
depends on.

## Assets / Resources

Record the prefabs, scenes, DT/DA assets and Addressables groups touched, with GUIDs and addresses kept.

## Build Configurations

Variants: the define symbols (`DEV_PROFILE`, `PRD_PROFILE`, `USE_FIREBASE`, `USE_APPSFLYER`) and Editor versus player behaviour (`#if UNITY_EDITOR` paths in the bootstrapper); say which one each behaviour was proved on.

## Alternatives Considered

1. Option.
