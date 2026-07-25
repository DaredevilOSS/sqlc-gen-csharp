#!/usr/bin/env bash
# generate-release-notes.sh
#
# Shared release notes generator for sqlc-gen-csharp.
# Only includes PRs with major/minor/patch labels (excludes skip-release).
#
# Usage:
#   OWNER=... REPO=... GH_TOKEN=... ./scripts/generate-release-notes.sh \
#     <prev-tag> <new-version> <sha256> [--file <path>]

set -euo pipefail

: "${OWNER:?}" "${REPO:?}" "${GH_TOKEN:?}"

PREVIOUS_TAG="${1:?Usage: generate-release-notes.sh <prev-tag> <new-version> <sha256> [--file <path>]}"
NEW_VERSION="${2:?}"
SHA256_HASH="${3:?}"
OUTPUT_FILE=""
shift 3

while [ "$#" -gt 0 ]; do
  case "$1" in
    --file) OUTPUT_FILE="$2"; shift 2 ;;
    *) echo "Unknown option: $1" >&2; exit 1 ;;
  esac
done

# Get commits between tags (match PR numbers like #123)
if [ -z "$PREVIOUS_TAG" ]; then
  COMMITS=$(git log --oneline --grep="#[0-9]" -E 2>/dev/null || echo "")
else
  COMMITS=$(git log "${PREVIOUS_TAG}..HEAD" --oneline --grep="#[0-9]" -E 2>/dev/null || echo "")
fi

CHANGELOG_ITEMS=()
CONTRIBUTORS=()

while IFS= read -r commit; do
  [ -z "$commit" ] && continue
  PR_NUM=$(echo "$commit" | grep -oE '#[0-9]+' | tr -d '#' || echo "")
  [ -z "$PR_NUM" ] && continue

  PR_JSON=$(gh api "repos/$OWNER/$REPO/pulls/$PR_NUM" \
    --jq '{title: .title, author: .user.login, labels: [.labels[].name]}' 2>/dev/null || echo '{}')
  PR_TITLE=$(echo "$PR_JSON" | jq -r '.title // empty')
  PR_AUTHOR=$(echo "$PR_JSON" | jq -r '.author // empty')
  [ -z "$PR_TITLE" ] && continue

  HAS_LABEL=false
  SKIP_RELEASE=false
  while IFS= read -r label; do
    [ -z "$label" ] && continue
    case "$label" in
      major|minor|patch) HAS_LABEL=true ;;
      skip-release) SKIP_RELEASE=true ;;
    esac
  done <<< "$(echo "$PR_JSON" | jq -r '.labels[] // empty')"

  if $HAS_LABEL && ! $SKIP_RELEASE; then
    CHANGELOG_ITEMS+=("- $PR_TITLE (#$PR_NUM)")
    CLEAN_AUTHOR="${PR_AUTHOR#app/}"
    FOUND=false
    for c in "${CONTRIBUTORS[@]}"; do
      [ "$c" = "$CLEAN_AUTHOR" ] && FOUND=true && break
    done
    $FOUND || CONTRIBUTORS+=("$CLEAN_AUTHOR")
  fi
done <<< "$COMMITS"

# Write output
if [ -n "$OUTPUT_FILE" ]; then
  exec > "$OUTPUT_FILE"
fi

echo "## Release ${NEW_VERSION}"
echo ""
echo "Release sha256 is \`${SHA256_HASH}\`"
echo ""
echo "## Configuration example"
echo '```yaml'
echo "version: '2'"
echo "plugins:"
echo "- name: csharp"
echo "  wasm:"
echo "    url: https://github.com/${OWNER}/${REPO}/releases/download/${NEW_VERSION}/sqlc-gen-csharp.wasm"
echo "    sha256: ${SHA256_HASH}"
echo '```'
echo ""
echo "## Changelog"
for item in "${CHANGELOG_ITEMS[@]}"; do
  echo "$item"
done
echo ""
echo "## Contributors"
for author in "${CONTRIBUTORS[@]}"; do
  echo "* @${author}"
done
