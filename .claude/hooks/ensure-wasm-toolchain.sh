#!/usr/bin/env bash
# Installs the WASI SDK and the wasi-experimental dotnet workload. Idempotent; safe to run
# manually (it blocks until the toolchain is ready). Needed for `make setup-wasm-plugin`.
set -euo pipefail

ROOT="${CLAUDE_PROJECT_DIR:-$(cd "$(dirname "$0")/../.." && pwd)}"
# shellcheck source=/dev/null
source "$ROOT/scripts/tool-versions.env"

export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}" DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1
export PATH="$DOTNET_ROOT:$PATH"
WASI_SDK_PATH="${WASI_SDK_PATH:-$HOME/.local/wasi-sdk}"
LOCK="${TMPDIR:-/tmp}/wasm-toolchain.lock"

exec 9>"$LOCK"
flock 9

if [ ! -x "$WASI_SDK_PATH/bin/clang" ]; then
  tmp="$(mktemp -d)"
  folder="wasi-sdk-${WASI_SDK_VERSION}.0-x86_64-linux"
  curl -fsSL "https://github.com/WebAssembly/wasi-sdk/releases/download/wasi-sdk-${WASI_SDK_VERSION}/${folder}.tar.gz" -o "$tmp/wasi.tgz"
  tar -xzf "$tmp/wasi.tgz" -C "$tmp"
  rm -rf "$WASI_SDK_PATH"
  mkdir -p "$(dirname "$WASI_SDK_PATH")"
  mv "$tmp/$folder" "$WASI_SDK_PATH"
  rm -rf "$tmp"
fi

if ! dotnet workload list 2>/dev/null | grep -q wasi-experimental; then
  dotnet workload install wasi-experimental
fi

echo "wasm toolchain ready (WASI_SDK_PATH=$WASI_SDK_PATH)"
