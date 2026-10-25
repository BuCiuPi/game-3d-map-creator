# API Codegen — Project Configuration

Status: **unconfigured by design**.

The template ships without a backend, authentication flow, generated client, or
networking package. API code generation is dormant until a derived game has a
real endpoint consumer and explicitly completes this file.

## Activation Gate

Before generating API code, all values below must be concrete:

```text
status                       = unconfigured
package_id                   = TODO(project)
package_git_url              = TODO(project)
package_commit_sha           = TODO(project)
runtime_assembly             = TODO(project)
request_assembly             = TODO(project)
request_namespace            = TODO(project)
transport_type               = TODO(project)
request_base_type            = TODO(project)
response_base_type           = TODO(project)
result_type                  = TODO(project)
error_type                   = TODO(project)
token_contract               = TODO(project)
composition_root             = TODO(project)
contract_source              = TODO(project): OpenAPI URL or captured samples
base_url_source              = TODO(project)
test_assembly                = TODO(project)
```

If any required value is still `TODO(project)`, stop. Do not infer the current
HyperEel API from a historical package, Cricket code, or the inactive templates
under `.claude/skills/api-codegen/References/`.

## How A Derived Project Activates It

1. Follow [`docs/architecture/networking-on-demand.md`](../docs/architecture/networking-on-demand.md).
2. Add the networking capability as a direct Git UPM dependency pinned to a
   commit SHA.
3. Let Unity resolve the package and regenerate `Packages/packages-lock.json`.
4. Inspect the mounted package version and record its actual assemblies, public
   types, namespaces, request conventions, result/error model, and auth seam in
   this file.
5. Create a feature-owned runtime assembly and test assembly in the derived
   project; do not put game APIs in `game.core`.
6. Set `status = configured`, then run the API skills.

`com.paul.*`, `Paul.*`, local `file:` dependencies, and the retired package
submodules are not valid defaults for this repository.
