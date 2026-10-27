#!/usr/bin/env bash
# Builds sqlc at the pinned commit twice: as is (bin/sqlc-upstream) and with the patches in this directory (bin/sqlc).
# Usage: scripts/sqlc-patches/build_sqlc.sh [output dir, default ./bin]
set -euo pipefail

PATCHES_DIR="$(cd "$(dirname "$0")" && pwd)"
OUT_DIR="$(mkdir -p "${1:-bin}" && cd "${1:-bin}" && pwd)"
REF="$(cat "$PATCHES_DIR/SQLC_REF")"
SRC="$(mktemp -d)"

git clone --quiet https://github.com/sqlc-dev/sqlc "$SRC"
git -C "$SRC" checkout --quiet "$REF"
(cd "$SRC" && go build -o "$OUT_DIR/sqlc-upstream" ./cmd/sqlc)
git -C "$SRC" apply "$PATCHES_DIR"/*.patch
(cd "$SRC" && go build -o "$OUT_DIR/sqlc" ./cmd/sqlc)
rm -rf "$SRC"
echo "built $OUT_DIR/sqlc-upstream and $OUT_DIR/sqlc (sqlc $REF + patches)"
