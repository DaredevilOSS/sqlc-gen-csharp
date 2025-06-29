#!/bin/bash

set -e

CI_YAML="sqlc.ci.yaml"
LOCAL_YAML="sqlc.local.yaml"
TMP_YAML="${LOCAL_YAML}.tmp"

cp "$LOCAL_YAML" "$TMP_YAML"

sql_count=$(yq '.sql | length' "$CI_YAML")
for ((i=0; i<sql_count; i++)); do
  codegen_count=$(yq ".sql[$i].codegen | length" "$CI_YAML")
  for ((j=0; j<codegen_count; j++)); do
    yq -i ".sql[$i].codegen[$j].options = (load(\"$CI_YAML\") | .sql[$i].codegen[$j].options)" "$TMP_YAML"
    yq -i ".sql[$i].codegen[$j].options.debugRequest = true" "$TMP_YAML"
  done
done

mv "$TMP_YAML" "$LOCAL_YAML"
echo "Options sections have been copied from $CI_YAML to $LOCAL_YAML and debugRequest: true has been set."