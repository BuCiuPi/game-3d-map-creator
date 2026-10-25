# SDK And Define Inventory

Read on 2026-10-25 after applying the neutral identity baseline.

## Owned Consumers

Codebase graph searches under `Assets/game.*` returned zero matches for
Firebase, AppsFlyer, AppLovin, Google Mobile Ads, MAX, `USING_FIREBASE`, and
`USING_APPSFLYER`. The neutral bootstrap, UI, and data core therefore have no
consumer that justifies those integrations.

## C7 Disposition

| Surface | Result |
| --- | --- |
| Jigsaw Firebase app configs | removed |
| `USING_FIREBASE`, AppsFlyer, ads, MAX, analytics and profile symbols | removed and covered by EditMode |
| Android resolver bundle ID | retired with the resolver in C8.1 |
| Firebase / resolver / ad SDK source and generated repositories | removed in C8.1 after zero external GUID references and zero runtime consumers were confirmed |
| Plugin-generated capability symbols | retained while their source plugin remains |

## C8 Boundary

C8.1 mapped every vendored service root, native plugin, resolver dependency,
generated local repository, manifest package, and ProjectSettings side effect.
The entire zero-consumer cohort was removed with its platform/native/generated
companions. No API or SDK is re-added until a derived project has a real
consumer. Remaining non-service tools are the separate C8.2 audit scope.
