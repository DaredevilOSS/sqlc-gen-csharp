#!/bin/bash

set -e

CI_YAML="sqlc.ci.yaml"
REQUESTS_YAML="sqlc.request.generated.yaml"
LOCAL_YAML="sqlc.local.generated.yaml"
TMP_REQUESTS_YML="${REQUESTS_YAML}.tmp"
TMP_LOCAL_YML="${LOCAL_YAML}.tmp"

cp "$REQUESTS_YAML" "$TMP_REQUESTS_YML"
cp "$LOCAL_YAML" "$TMP_LOCAL_YML"

sql_count=$(yq '.sql | length' "$CI_YAML")
for ((i=0; i<sql_count; i++)); do
  codegen_count=$(yq ".sql[$i].codegen | length" "$CI_YAML")
  for ((j=0; j<codegen_count; j++)); do
    yq -i ".sql[$i].codegen[$j].options = (load(\"$CI_YAML\") | .sql[$i].codegen[$j].options)" "$TMP_REQUESTS_YML"
    yq -i ".sql[$i].codegen[$j].options = (load(\"$CI_YAML\") | .sql[$i].codegen[$j].options)" "$TMP_LOCAL_YML"
    yq -i ".sql[$i].codegen[$j].options.debugRequest = true" "$TMP_REQUESTS_YML"
  done
done

mv "$TMP_REQUESTS_YML" "$REQUESTS_YAML"
echo "Options sections have been copied from $CI_YAML to $REQUESTS_YAML and debugRequest: true has been set."
mv "$TMP_LOCAL_YML" "$LOCAL_YAML"
echo "Options sections have been copied from $CI_YAML to $LOCAL_YAML."