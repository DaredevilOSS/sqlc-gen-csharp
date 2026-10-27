#!/usr/bin/env bash
# SessionStart hook for Claude Code cloud sessions: installs dotnet, sqlc and yq (blocking, ~30s)
# and, in the background, the WASM toolchain (WASI SDK + wasi-experimental workload) plus the
# PostgreSQL/MySQL servers needed by the end-to-end tests.
# Versions: dotnet <- global.json, everything else <- scripts/tool-versions.env.
set -euo pipefail

if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

ROOT="${CLAUDE_PROJECT_DIR:-$(cd "$(dirname "$0")/../.." && pwd)}"
# shellcheck source=/dev/null
source "$ROOT/scripts/tool-versions.env"
DOTNET_SDK_VERSION="$(sed -n 's/.*"version"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' "$ROOT/global.json" | head -n1)"

DOTNET_ROOT="$HOME/.dotnet"
BIN_DIR="$HOME/.local/bin"
WASI_SDK_PATH="$HOME/.local/wasi-sdk"
LOG_DIR="$HOME/.cache/claude-hook"
mkdir -p "$BIN_DIR" "$LOG_DIR"

export DOTNET_ROOT DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1
export PATH="$BIN_DIR:$DOTNET_ROOT:$PATH"

# --- .NET SDK ---------------------------------------------------------------
if ! "$DOTNET_ROOT/dotnet" --list-sdks 2>/dev/null | grep -q "^${DOTNET_SDK_VERSION} "; then
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o "$LOG_DIR/dotnet-install.sh"
  bash "$LOG_DIR/dotnet-install.sh" --version "$DOTNET_SDK_VERSION" --install-dir "$DOTNET_ROOT"
fi

# --- sqlc -------------------------------------------------------------------
if ! "$BIN_DIR/sqlc" version 2>/dev/null | grep -q "${SQLC_VERSION}"; then
  tmp="$(mktemp -d)"
  curl -fsSL "https://github.com/sqlc-dev/sqlc/releases/download/v${SQLC_VERSION}/sqlc_${SQLC_VERSION}_linux_amd64.tar.gz" -o "$tmp/sqlc.tgz"
  tar -xzf "$tmp/sqlc.tgz" -C "$tmp" sqlc
  install -m 0755 "$tmp/sqlc" "$BIN_DIR/sqlc"
  rm -rf "$tmp"
fi

# --- yq (mikefarah; scripts/sync_sqlc_options.sh needs it, not the python yq) --
if ! "$BIN_DIR/yq" --version 2>/dev/null | grep -q "v${YQ_VERSION}"; then
  curl -fsSL "https://github.com/mikefarah/yq/releases/download/v${YQ_VERSION}/yq_linux_amd64" -o "$BIN_DIR/yq"
  chmod +x "$BIN_DIR/yq"
fi

# --- persist environment for the session --------------------------------------
if [ -n "${CLAUDE_ENV_FILE:-}" ]; then
  {
    echo "export DOTNET_ROOT=\"$DOTNET_ROOT\""
    echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1"
    echo "export DOTNET_NOLOGO=1"
    echo "export WASI_SDK_PATH=\"$WASI_SDK_PATH\""
    echo "export PATH=\"$BIN_DIR:$DOTNET_ROOT:\$PATH\""
  } >> "$CLAUDE_ENV_FILE"
fi

# --- WASM toolchain (background, only needed for `make setup-wasm-plugin`) ----
# Idempotent and lock-guarded: `.claude/hooks/ensure-wasm-toolchain.sh` waits for / finishes it.
setsid nohup "$ROOT/.claude/hooks/ensure-wasm-toolchain.sh" \
  > "$LOG_DIR/wasm-toolchain.log" 2>&1 < /dev/null &

# --- PostgreSQL + MySQL for the end-to-end tests (background; no Docker daemon in cloud sessions) --
# Idempotent: installs the servers once (cached) and starts them on every session.
if [ "$(id -u)" = "0" ]; then
  setsid nohup "$ROOT/.claude/hooks/setup-test-databases.sh" \
    > "$LOG_DIR/test-databases.log" 2>&1 < /dev/null &
fi

echo "dotnet $("$DOTNET_ROOT/dotnet" --version), sqlc $("$BIN_DIR/sqlc" version), $("$BIN_DIR/yq" --version)"
echo "WASM toolchain and test databases installing in background (logs: $LOG_DIR/*.log)"
