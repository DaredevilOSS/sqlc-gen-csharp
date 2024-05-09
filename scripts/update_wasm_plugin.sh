#!/usr/bin/env bash

set -e

if [ "$GITHUB_ACTIONS" = "true" ]; then
    SOURCE_WASM_FILE="${SOURCE_WASM_FILE_UBUNTU}"
else
    SOURCE_WASM_FILE="${SOURCE_WASM_FILE}"
fi
mkdir -p dist
cp "${SOURCE_WASM_FILE}" dist/plugin.wasm
echo "WASM filesize:" && du -sh dist/plugin.wasm

PLUGIN_SHA=$(shasum -a 256 dist/plugin.wasm | awk '{ print $1 }')
yq -i ".plugins[0].wasm.sha256 = \"${PLUGIN_SHA}\"" sqlc.wasm.yaml