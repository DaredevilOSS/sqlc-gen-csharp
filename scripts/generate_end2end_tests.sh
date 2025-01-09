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
  export IS_LEGACY=false
  export TEST_CLASS_NAME="$1"
  local TEST_FILENAME="${TEST_CLASS_NAME}.generated.cs"
  echo "generating EndToEndTests/$TEST_FILENAME..."
  dotnet run --project ./EndToEndScaffold/EndToEndScaffold.csproj > ./EndToEndTests/"$TEST_FILENAME"
}

generate_legacy() {
  export IS_LEGACY=true
  export TEST_CLASS_NAME="$1"
  local TEST_FILENAME="${TEST_CLASS_NAME}.generated.cs"
  echo "generating EndToEndTestsLegacy/$TEST_FILENAME..."
  dotnet run --project ./EndToEndScaffold/EndToEndScaffold.csproj > ./EndToEndTestsLegacy/"$TEST_FILENAME"
}

for target in "${TARGETS[@]}"; do
    generate "$target"
    generate_legacy "$target"
done