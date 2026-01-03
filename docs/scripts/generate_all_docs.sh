#!/usr/bin/env bash

set -e

./docs/scripts/generate_quickstart.sh > docs/02_Quickstart.md
dotnet run --project docs/ExamplesDocGen/ExampleDocGen.csproj > docs/09_Examples.md
cat docs/0*.md > README.md