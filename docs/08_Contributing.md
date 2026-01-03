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
In order to create a release you need to add `[release]` somewhere in your commit message when merging to master.

### Version bumping (built on tags)
By default, the release script will bump the patch version. Adding `[release]` to your commit message results in a new tag with `v[major].[minor].[patch]+1`. 
- Bump `minor` version by adding `[minor]` to your commit message resulting in a new tag with `v[major].[minor]+1.0` <br/>
- Bump `major` version by adding `[major]` to your commit message resulting in a new tag with `v[major]+1.0.0` <br/>

### Release structure
The new created tag will create a draft release with it, in the release there will be the wasm plugin embedded in the release. <br/>

