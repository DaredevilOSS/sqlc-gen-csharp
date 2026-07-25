# Contributing
## Local plugin development
### Prerequisites
Make sure that the following applications are installed and added to your path.

Follow the instructions in each of these:
- Dotnet CLI - [Dotnet Installation](https://github.com/dotnet/sdk) - use version `.NET 8.0 (latest)` <br/>
- Buf build - [Buf Build](https://buf.build/docs/installation) <br/>
- WASM (follow this guide) - [WASM libs](https://www.strathweb.com/2023/09/dotnet-wasi-applications-in-net-8-0/)

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
make protobuf-generate
```

### Generating code
SQLC utilizes our process / WASM plugin to generate code:
```
make sqlc-generate-process
make sqlc-generate-wasm
```

### Testing generated code
Testing the SQLC generated code via a predefined flow:
```
make test-process-plugin
make test-wasm-plugin
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

