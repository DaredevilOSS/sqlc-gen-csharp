#!/usr/bin/env bash

set -e
source .env

if [ "${GITHUB_ACTIONS}" = "true" ]; then
    WASM_FILE="${SOURCE_WASM_FILE_UBUNTU}"
else
    WASM_FILE="${SOURCE_WASM_FILE}"
fi

echo "WASM_FILE = ${WASM_FILE}"
mkdir -p dist
cp "${WASM_FILE}" dist/plugin.wasm
echo "WASM filesize:" && du -sh dist/plugin.wasm