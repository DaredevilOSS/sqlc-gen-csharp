#!/usr/bin/env bash

set -e

doc_file=$1

examples_cnt=$(yq ".sql | length" sqlc.ci.yaml)
examples_doc="# Examples"

for ((i = 0 ; i < "${examples_cnt}" ; i++ )); do 
    engine_name=$(yq ".sql[${i}].engine" sqlc.ci.yaml)
    schema_file=$(yq ".sql[${i}].schema" sqlc.ci.yaml)
    query_files=$(yq ".sql[${i}].queries" sqlc.ci.yaml)
    
    project_name=$(yq ".sql[${i}].codegen[0].out" sqlc.ci.yaml)
    test_class_name="${project_name/Example/"Tester"}"  
    examples_doc+="
## Engine \`${engine_name}\`: [${project_name}](../${project_name})

### [Schema](../${schema_file}) | [Queries](../${query_files}) | [End2End Test](../EndToEndTests/${test_class_name}.cs)

### Config
\`\`\`yaml
$(yq ".sql[${i}].codegen[${0}].options" sqlc.ci.yaml)
\`\`\`
"
done

echo "${examples_doc}" > "${doc_file}"