#!/usr/bin/env bash

set -e
source .env

target_dir=$1
if [ "${GITHUB_ACTIONS}" = "true" ]; then
    WASM_FILE="${SOURCE_WASM_FILE_UBUNTU}"
else
    WASM_FILE="${SOURCE_WASM_FILE}"
fi

echo "WASM_FILE = ${WASM_FILE}"
mkdir -p "${target_dir}" && cp "${WASM_FILE}" "${target_dir}/plugin.wasm"
echo "WASM filesize:" && du -sh "${target_dir}/plugin.wasm"