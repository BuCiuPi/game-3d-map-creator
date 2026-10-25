# Design

## Data / Models

No runtime data model changes. The removal boundary is the seven service roots
plus their root `.meta` files, the resolver cache, four unused Unity packages,
and two unused registry scopes.

## Flow And Wiring

The template boots exactly as before. There is no replacement service or no-op
facade. Derived features mount a service through pinned Git UPM or the provider's
accepted package only when the first consumer exists.

## Contract Surface

The permanent invariant is absence, enforced by `TemplateBoundaryTests`:

```text
no Firebase / EDM4U / GeneratedLocalRepo / mobile service plugin roots
no custom Android service build templates
no IAP or Unity cloud-service manifest entry
no AppLovin or Google EDM registry scope
```

## Assets / Resources

Candidate roots own 145 GUIDs. A repository-wide serialized scan found zero
references from outside the cohort before deletion. No Addressable entry or
game-owned asset is moved.

## Build Configurations

Deleting `Assets/Plugins/{Android,iOS,tvOS}` restores Unity-owned platform build
files. Unity import must serialize all custom Android-template flags off. The
active Editor/Standalone proof remains the only local compile target; device
builds are deferred until a platform is selected.

## Alternatives Considered

1. Keep SDKs disabled behind symbols: rejected because native binaries, resolver
   state, build templates, and repository weight remain inherited by every game.
2. Keep Firebase/ads as examples: rejected because product service configuration
   is not a neutral sample.
3. Prune every unused third-party tool in the same change: deferred so mobile
   service removal has a bounded dependency and fresh-clone proof.
