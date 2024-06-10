#!/usr/bin/env bash

set -e
source .env

RUNNER_OS=$(uname -s)
if [ "${RUNNER_OS}" == "Darwin" ]; then
    WASM_FILE="${SOURCE_WASM_FILE_MACOS}"
elif [ "${RUNNER_OS}" == "Linux" ]; then
    WASM_FILE="${SOURCE_WASM_FILE_UBUNTU}"
fi

echo "WASM_FILE = ${WASM_FILE}"
mkdir -p dist
cp "${WASM_FILE}" dist/plugin.wasm
echo "WASM filesize:" && du -sh dist/plugin.wasm