#!/usr/bin/env bash

set -e

cd $(git rev-parse --show-toplevel)/benchmark

latest_tag=$(git describe --tags --abbrev=0)
plugin_url="https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/${latest_tag}/sqlc-gen-csharp.wasm"
yq -i ".plugins[0].wasm.url = \"${plugin_url}\"" sqlc.yaml

curl -OL "${plugin_url}"
plugin_sha=$(shasum -a 256 sqlc-gen-csharp.wasm | awk '{ print $1 }')
yq -i ".plugins[0].wasm.sha256 = \"${plugin_sha}\"" sqlc.yaml