#!/usr/bin/env bash

set -e

doc_file="docs/$1"

examples_cnt=$(yq ".sql | length" sqlc.ci.yaml)
contents="[comment]: <> (do not edit - CI auto-generated)
# Examples"

for ((i = 0 ; i < "${examples_cnt}" ; i++ )); do 
    engine_name=$(yq ".sql[${i}].engine" sqlc.ci.yaml)
    schema_file=$(yq ".sql[${i}].schema" sqlc.ci.yaml)
    query_files=$(yq ".sql[${i}].queries" sqlc.ci.yaml)
    output_directory=$(yq ".sql[${i}].codegen[0].out" sqlc.ci.yaml)
    
    project_name="${output_directory/examples\//}"
    if [[ "$project_name" == *"Legacy"* ]]; then
      test_project="LegacyEndToEndTests"
      test_class_name="${project_name/Example/"Tester"}"
      test_class_name="${test_class_name/Legacy/""}"
    else
      test_project="EndToEndTests"
      test_class_name="${project_name/Example/"Tester"}"
    fi
    
    contents+="
## Engine \`${engine_name}\`: [../${project_name}](${output_directory})

### [Schema](../${schema_file}) | [Queries](../${query_files}) | [End2End Test](../${test_project}/${test_class_name}.cs)

### Config
\`\`\`yaml
$(yq ".sql[${i}].codegen[${0}].options" sqlc.ci.yaml)
\`\`\`
"
done

echo "${contents}" > "${doc_file}"