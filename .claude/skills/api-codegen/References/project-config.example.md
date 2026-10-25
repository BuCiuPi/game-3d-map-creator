# API Codegen — Project Config (example)

<!-- Ported from cricket-city-unity .claude/skills/api-codegen/References/project-config.example.md @ 8da403c97; rewritten for com.paul.common.networking @ beb0af1. -->

Copy this file to `.claude/api-codegen-config.md` and replace every value with
one read from the repository. Cite the file each value came from. A value only a
real game has (spec URL, base URL, auth) stays `TODO(project): …` until the game
has it; `api-codegen` stops on the files that need it.

## Precondition

```
networking_package_mounted = yes | no   # "com.paul.common.networking" in Packages/manifest.json
request_assembly_references_networking = yes | no
```

## Contract source

```
samples_path     = docs/api/samples/            # captured responses, if any
spec_url         = https://<host>/api/schema/   # Swagger/OpenAPI, if any
dictionary_paths = feature.board, feature.board.*   # payload paths whose keys are data
error_signal     = message | error_code | both  # which one the backend actually fills
```

## Namespaces

```
request_namespace = MyGame.Feature.Api.Requests
data_namespace    = MyGame.Feature.Api.Data
gateway_namespace = MyGame.Feature.Api
model_namespace   = MyGame.Feature.{Domain}         # the slice folder
mapping_namespace = MyGame.Feature.{Domain}
test_namespace    = MyGame.Feature.Tests.EditMode.Api
```

## Output Paths (relative to repo root)

These are the keys `SKILL.md`'s `{{placeholders}}` read.

```
request_output_path = Assets/MyGame.Feature/Runtime/Api/Requests
data_output_path    = Assets/MyGame.Feature/Runtime/Api/Data
gateway_path        = Assets/MyGame.Feature/Runtime/Api
model_output_path   = Assets/MyGame.Feature/Runtime/{Domain}
mapping_output_path = Assets/MyGame.Feature/Runtime/{Domain}
mock_json_path      = (only if the project keeps mock responses on disk)
test_path           = Assets/MyGame.Feature/Tests/EditMode/Api
```

## Framework

Tells the generator the exact framework shape so it never hardcodes a namespace.

```
transport          = ApiClient                     # Paul.Base.Networking.Runtime
gateway_base_class = (none)                        # each gateway holds ApiClient + private SendAsync
result_type        = Result<T>                     # Paul.Game.Shared.Runtime.Networking
request_source_generator = none                    # override GetRequestMethod/GetPath/GetResources/GetParameters/GetHeaders

# The exact `using` directives for request files, inside the namespace.
framework_usings =
    using Newtonsoft.Json;
    using Paul.Base.Networking.Runtime;
    using Paul.Base.Networking.Runtime.Request.Base;
    using Paul.Base.Networking.Runtime.Request.Base.Interfaces;
    using Paul.Game.Shared.Runtime.Networking;      # Const.JwtAuthKeyHeader

requires_json_object_optin = true                  # SECURITY — every IJsonDataRequest
debug_define               = PRD_PROFILE           # debug endpoints sit in #if !PRD_PROFILE
```

## Transport configuration

```
base_url      = TODO(project): the API base URL per environment; ends with '/' if it carries a path
base_url_source = TODO(project): where the code reads it (a DA_ asset, a const, a profile define)
auth          = TODO(project): who calls ApiContextBuilder.SetToken, and when
settings_asset = Assets/Resources/Paul.Core.Networking/NetworkCommonSettings.asset   # must exist
```

## Areas

One interface per URL prefix, with its registration scope.

```
IFeatureApi = feature/api/v1/...   scope: scene    (carries a session id)
IAccountApi = account/api/v1/...   scope: LobbyLifeTimeScope singleton
```

## Reference Files (style source of truth)

Point to real files so the codegen matches the existing style. Mark an entry
`(none yet)` until the first endpoint lands; the templates are the style until then.

```
example_request = Assets/MyGame.Feature/Runtime/Api/Requests/FeatureRequests.cs
example_data    = Assets/MyGame.Feature/Runtime/Api/Data/FeatureStateApiData.cs
example_mapping = Assets/MyGame.Feature/Runtime/Feature/FeatureApiDataToModelExtension.cs
example_gateway = Assets/MyGame.Feature/Runtime/Api/FeatureApiGateway.cs
example_test    = Assets/MyGame.Feature/Tests/EditMode/Api/FeatureRequestTests.cs
```

## Assemblies

```
request_assembly = mygame.feature.runtime
test_assembly    = mygame.feature.tests.editmode
```
