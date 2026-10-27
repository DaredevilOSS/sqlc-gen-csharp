#!/bin/bash

set -e

# sync_pair <ci yaml> <requests yaml> <local yaml>: copies every .sql entry of the ci yaml (which uses the WASM
# plugin) into the generated yamls (which use the process plugin), with debugRequest: true in the requests yaml.
sync_pair() {
  local CI_YAML="$1"
  local REQUESTS_YAML="$2"
  local LOCAL_YAML="$3"
  local TMP_REQUESTS_YML="${REQUESTS_YAML}.tmp"
  local TMP_LOCAL_YML="${LOCAL_YAML}.tmp"

  cp "$REQUESTS_YAML" "$TMP_REQUESTS_YML"
  cp "$LOCAL_YAML" "$TMP_LOCAL_YML"
  local sql_count
  sql_count=$(yq '.sql | length' "$CI_YAML")
  for ((i=0; i<sql_count; i++)); do
    yq -i ".sql[$i] = (load(\"$CI_YAML\") | .sql[$i])" "$TMP_REQUESTS_YML"
    yq -i ".sql[$i] = (load(\"$CI_YAML\") | .sql[$i])" "$TMP_LOCAL_YML"
    yq -i ".sql[$i].codegen[0].options.debugRequest = true" "$TMP_REQUESTS_YML"
  done
  # drop the entries that were removed from the ci yaml
  yq -i ".sql |= .[:$sql_count]" "$TMP_REQUESTS_YML"
  yq -i ".sql |= .[:$sql_count]" "$TMP_LOCAL_YML"

  mv "$TMP_REQUESTS_YML" "$REQUESTS_YAML"
  echo "Options sections have been copied from $CI_YAML to $REQUESTS_YAML and debugRequest: true has been set."
  mv "$TMP_LOCAL_YML" "$LOCAL_YAML"
  echo "Options sections have been copied from $CI_YAML to $LOCAL_YAML."
}

sync_pair sqlc.ci.yaml sqlc.request.generated.yaml sqlc.local.generated.yaml
sync_pair sqlc.mssql.ci.yaml sqlc.mssql.request.generated.yaml sqlc.mssql.local.generated.yaml
