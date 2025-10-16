#!/usr/bin/env bash

set -ex

TARGETS=(
  "MySqlConnectorTester"
  "MySqlConnectorDapperTester"
  "NpgsqlTester"
  "NpgsqlDapperTester"
  "SqliteTester"
  "SqliteDapperTester"
)
  
generate() {
  local TEST_FILENAME="${1}.generated.cs"
  echo "generating EndToEndTests/$TEST_FILENAME..."
  IS_LEGACY=false TEST_CLASS_NAME="$1" dotnet run --project ./end2end/EndToEndScaffold/EndToEndScaffold.csproj > ./end2end/EndToEndTests/"$TEST_FILENAME"
}

generate_legacy() {
  local TEST_FILENAME="${1}.generated.cs"
  echo "generating EndToEndTestsLegacy/$TEST_FILENAME..."
  IS_LEGACY=true TEST_CLASS_NAME="$1" dotnet run --project ./end2end/EndToEndScaffold/EndToEndScaffold.csproj > ./end2end/EndToEndTestsLegacy/"$TEST_FILENAME"
}

for target in "${TARGETS[@]}"; do
    generate "$target"
    generate_legacy "$target"
done