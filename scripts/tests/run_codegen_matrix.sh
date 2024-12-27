##!/bin/bash
#
#set -e
#source .env
#
#config_file=$1
#
#examples=()
#while IFS= read -r example; do
#    examples+=("${example}")
#done < <(dotnet sln list | grep Example | xargs -n 1 dirname)
#
#change_config_to_default() {
#  generate_csproj="true"
#  target_framework="net8.0"
#  for ((i=0; i<${#examples[@]}; i++)); do
#    echo "Changing configuration back to default for project ${examples[i]}" 
#    yq -i "
#      .sql[${i}].codegen[0].options.generateCsproj = ${generate_csproj} |
#      .sql[${i}].codegen[0].options.targetFramework = \"${target_framework}\"
#    " "${config_file}"
#    echo "${examples[i]} codegen config:" && yq ".sql[${i}].codegen[0]" "${config_file}"
#  done
#}
#
#generate_csproj="true false"
#target_framework="net8.0 netstandard2.0 netstandard2.1"
#
#for x in $generate_csproj; do
#    for y in $target_framework; do
#        echo "Running with generate-csproj=$x, target-framework=$y"
#        ./scripts/tests/run_codegen.sh "${config_file}" "$x" "$y"
#        echo "---Finished [generate-csproj=$x, target-framework=$y]---"
#    done
#done
#
#change_config_to_default