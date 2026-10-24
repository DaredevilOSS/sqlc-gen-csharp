# Contributing
## Local plugin development
### Prerequisites
Make sure that the following applications are installed and added to your path.

Follow the instructions in each of these:
- Dotnet CLI - [Dotnet Installation](https://github.com/dotnet/sdk) - the SDK version pinned in `global.json` (`.NET 10`) <br/>
- Buf build - [Buf Build](https://buf.build/docs/installation) <br/>
- sqlc - [sqlc Installation](https://docs.sqlc.dev/en/latest/overview/install.html) <br/>
- Docker - for running the end-to-end tests against real PostgreSQL / MySQL instances <br/>
- WASM build tooling (only needed for building the WASM plugin):
  - the `wasi-experimental` workload - `dotnet workload install wasi-experimental`
  - [WASI SDK](https://github.com/WebAssembly/wasi-sdk/releases) version `25`, extracted somewhere and exposed through the `WASI_SDK_PATH` environment variable

### Target frameworks
Two different .NET versions are involved, and they are independent of each other:
- The **plugin itself** (all projects except `examples`) targets `net10.0`.
- The **generated code** targets whatever `targetFramework` is configured - `netstandard2.0`, `netstandard2.1`, `net8.0` (default), `net9.0` or `net10.0`.
  The `examples` cover every supported value (see `sqlc.ci.yaml`) and are built and tested against real databases in CI.

### Why the WASM plugin targets `wasip1`
sqlc runs WASM plugins with [wazero](https://wazero.io/), which can load core modules (`wasm32-wasip1`) but not WebAssembly components.
The .NET 10 WASI SDK hardcodes `wasm32-unknown-wasip2` and links a component-model world, which produces a component that sqlc rejects (`invalid version header`).
`WasmRunner/WasmRunner.csproj` therefore overrides the compile and link steps to produce a `wasip1` core module.
These overrides rely on private item names of the WASI SDK, so after any .NET SDK or WASI SDK upgrade, verify the plugin with `make setup-wasm-plugin && sqlc -f sqlc.ci.yaml diff`.

## Pre-commit Setup
This repository uses [pre-commit](https://pre-commit.com/). To set up pre-commit hooks, run:

```bash
pip install pre-commit
pre-commit install
```

### Protobuf
SQLC protobuf are defined in sqlc-dev/sqlc repository.
Generating C# code from protocol buffer files:
```
make generate-protobuf
```

### Generating code
SQLC utilizes our process / WASM plugin to generate code:
```
make sqlc-generate        # process plugin - regenerates the examples and the codegen requests
make setup-wasm-plugin    # builds the WASM plugin into dist/plugin.wasm
sqlc -f sqlc.ci.yaml diff # verifies the WASM plugin generates exactly the committed examples
```

### Testing generated code
Testing the SQLC generated code via a predefined flow:
```
make unit-tests   # codegen and repository unit tests
make test-plugin  # regenerates everything, builds the solution and runs the end-to-end tests (needs Docker)
```

## Release flow

The release flow in this repo follows the semver conventions, building tag as `v[major].[minor].[patch]`.

### PR Gate

Every PR that modifies source code, build configuration, or similar must be labeled with one of: `major`, `minor`, `patch`, or `skip-release`.

A bot posts a status check (`release-assistant/requirements`) that blocks merging until a version label is present.

### Generate Release PR

When Build completes on `main`, the `gen-release-pr.yml` workflow runs and:

1. Aggregates all unreleased, labeled PRs since the last release tag.
2. Determines the release type from the labels (`major` > `minor` > `patch`).
3. Computes the new version from the latest tag.
4. Downloads the latest wasm artifact and computes its sha256.
5. Regenerates documentation (README, quickstart) with the new version and wasm sha.
6. Opens a `release-prep` PR with the regenerated docs.

### Release

When the `release-prep` PR is merged, `release.yml`:

1. Detects the `chore: prepare release vX` commit.
2. Downloads the wasm artifact from the triggering Build.
3. Reconstructs release notes from merged PR titles since the previous tag.
4. Creates the release (with tag) and uploads the wasm asset.

