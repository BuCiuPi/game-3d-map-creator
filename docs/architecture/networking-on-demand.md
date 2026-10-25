# Networking On Demand

The template installs no networking or backend package.
A derived project adds networking only with the first feature that calls an API
(decision `0008`).

## Git UPM Recipe

1. Select an accepted commit of the package repository; never track a branch in
   a project manifest.
2. Add the package to `Packages/manifest.json` with `?path=` before `#<sha>`:

   ```json
   {
     "dependencies": {
       "hypereel.base.networking-v2": "https://github.com/hype-reel/HypeBase.Core.git?path=/hypereel.base.networking-v2#<commit-sha>"
     }
   }
   ```

3. If that package needs another Git-hosted package, declare that package
   directly in the project's manifest at an accepted commit too.
   Unity Package Manager supports Git dependencies in project manifests, not
   transitive Git dependencies declared by another package.
4. Open Unity `6000.3.10f1` and let it regenerate
   `Packages/packages-lock.json`; commit both manifest and lockfile.
5. Inspect the installed package's current assemblies and public API before
   generating code.
   Fill `.claude/api-codegen-config.md` in the derived project; the template's
   unconfigured file is a hard stop, not a guessed contract.

The URL form and dependency limitation were checked against the Unity 6 Package
Manager Git dependency manual on 2026-10-25.

## Proof

| Change | Required proof |
| --- | --- |
| Add or update Git UPM package | clean resolve, lockfile diff, compile |
| First endpoint | request serialization test, response projection test, gateway/service test |
| Composition wiring | EditMode resolution test and boot path |

Do not copy package source into `Assets/`, reintroduce a repository submodule,
or install auth/common/VFX packages speculatively.
