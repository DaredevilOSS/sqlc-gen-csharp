##!/usr/bin/env bash
#
#set -e
#
#mapfile -t examples < <(dotnet sln list | grep Example | xargs -n 1 dirname) # TODO standardize across scripts
#
#config_file=$1
#generate_csproj=$2
#
#change_config() {
#  for ((i=0; i<${#examples[@]}; i++)); do
#    echo "Changing configuration for project ${examples[i]}" 
#    yq -i "
#      .sql[${i}].codegen[0].options.generateCsproj = ${generate_csproj}
#    " "${config_file}"
#    echo "${examples[i]} codegen config:" && yq ".sql[${i}].codegen[0]" "${config_file}"
#  done
#}
#
#check_csproj_file() {
#  for example_dir in "${examples[@]}"
#  do
#    echo "Checking ${example_dir}.csproj file generated"
#    if [ ! -f "examples/${example_dir}/${example_dir}.csproj" ]; then
#      echo "Assertion failed: A .csproj file is not present in the directory ${example_dir}."
#      return 1
#    fi
#  done
#}
#
#check_csharp_file() {
#  for example_dir in "${examples[@]}"
#  do
#    echo "Checking ${example_dir}.csproj file generated"
#    if [ ! -f "examples/${example_dir}/${example_dir}.csproj" ]; then
#      echo "Assertion failed: A .csproj file is not present in the directory ${example_dir}."
#      return 1
#    fi
#  done
#}
#
#change_config
#sqlc -f "${config_file}" generate
#
#test_functions=("check_csproj_file")
#for test_function in "${test_functions[@]}"; do
#  ${test_function}
#  status_code=$?
#  if [ ${status_code} -ne 0 ]; then
#    echo "Function ${test_function} failed with status code ${status_code}"
#    exit "${status_code}"
#  fi
#  echo "Test ${test_function} passed"
#done
#generated_files_restore