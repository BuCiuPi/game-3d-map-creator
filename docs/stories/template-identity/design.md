# Design

## Data / Models

The baseline is one set of constants owned by
`TemplateProjectSettingsSetup`: company `Amihan`, product `Unity Template`,
identifier `com.amihan.unitytemplate`, version `0.1.0`, Android version code `1`,
and iOS build number `1`.

## Flow And Wiring

`Game Template/Project/Apply Neutral Identity` and the equivalent batch method
write supported settings through `PlayerSettings`, clear custom defines for the
four candidate target families, then save assets. It does not switch the active
build target.

## Contract Surface

The Editor method is the reproducible setup contract:

```text
Game.Bootstrap.Editor.Project.TemplateProjectSettingsSetup.ApplyNeutralIdentity
```

Derived projects replace the baseline identity and add product SDK/profile
define symbols only in the same change that introduces a real consumer.

## Assets / Resources

The unreferenced `Assets/Settings/Build Profiles/` Android profiles and their
`.meta` files are removed. No Addressable asset, address, group, or GUID changes.
The two Jigsaw Firebase app-config files are removed; SDK source remains for C8.

## Build Configurations

Product SDK/profile symbols are removed from every valid named target. Symbols
automatically maintained by retained plugins can remain until C8 removes those
plugins. The task does not create a build profile or select a platform. Compile,
EditMode, and PlayMode evidence runs in the Editor/Standalone environment.

## Alternatives Considered

1. Raw-edit `ProjectSettings.asset`: rejected because Unity owns its serialized
   schema and the change would not be reproducible.
2. Keep the Jigsaw Android profile as an example: rejected because it overrides
   identity, version, defines, and platform settings from another product.
3. Remove all third-party SDK folders now: deferred to C8 so dependency deletion
   has its own inventory and compile proof.
