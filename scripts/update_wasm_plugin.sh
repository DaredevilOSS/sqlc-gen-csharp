#!/usr/bin/env bash

set -e

if [ "$GITHUB_ACTIONS" = "true" ]; then
    WASM_FILE="${SOURCE_WASM_FILE_UBUNTU}"
    # need to install yq here for all operation systems
else
    source .env
    WASM_FILE="${SOURCE_WASM_FILE}"
fi
echo "WASM_FILE = ${WASM_FILE}"
mkdir -p dist
cp "${SOURCE_WASM_FILE}" dist/plugin.wasm
echo "WASM filesize:" && du -sh dist/plugin.wasm

PLUGIN_SHA=$(shasum -a 256 dist/plugin.wasm | awk '{ print $1 }')
yq -i ".plugins[0].wasm.sha256 = \"${PLUGIN_SHA}\"" sqlc.wasm.yaml