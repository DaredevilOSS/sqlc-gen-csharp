#!/usr/bin/env bash

set -e

SOURCE_WASM_FILE="SqlcGenCsharpWasm/bin/Release/net8.0/wasi-wasm/AppBundle/SqlcGenCsharpWasm.wasm"
cp "${SOURCE_WASM_FILE}" dist/plugin.wasm
echo "WASM filesize:" && du -sh dist/plugin.wasm

PLUGIN_SHA=$(shasum -a 256 dist/plugin.wasm | awk '{ print $1 }')
yq -i ".plugins[0].wasm.sha256 = \"${PLUGIN_SHA}\"" sqlc.wasm.yaml