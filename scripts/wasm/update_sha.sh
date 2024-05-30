#!/usr/bin/env bash

plugin_sha=$(shasum -a 256 dist/plugin.wasm | awk '{ print $1 }')
yq -i ".plugins[0].wasm.sha256 = \"${plugin_sha}\"" sqlc.wasm.yaml