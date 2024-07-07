#!/usr/bin/env bash

set -ex
source .env

mapfile -t examples < <(dotnet sln list | grep Example | xargs -n 1 dirname) # TODO standardize across scripts

config_file=$1
generate_csproj=$2
target_framework=$3

generated_files_cleanup() {
  for example_dir in "${examples[@]}"
  do
    echo "Deleting .cs files in ${example_dir}"
    find "${example_dir}/" -type f -name "*.cs" -exec rm -f {} \;
    if [ "${generate_csproj}" = "true" ]; then
      echo "Deleting .csproj file" && rm "${example_dir}/${example_dir}.csproj"
    fi
  done
}

change_config() {
  for ((i=0; i<${#examples[@]}; i++)); do
    echo "Changing configuration for project ${example_dir}" 
    yq -i "
      .sql[${i}].codegen[0].options.generateCsproj = ${generate_csproj} |
      .sql[${i}].codegen[0].options.targetFramework = \"${target_framework}\"
    " "${config_file}"
    echo "${examples[i]} codegen config:" && yq ".sql[${i}].codegen[0]" "${config_file}"
  done
}

check_csproj_file() {
  for example_dir in "${examples[@]}"
  do
    echo "Checking ${example_dir}.csproj file generated"
    if [ ! -f "${example_dir}/${example_dir}.csproj" ]; then
      echo "Assertion failed: A .csproj file is not present in the directory ${example_dir}."
      return 1
    fi
  done
}

check_project_compiles() {
  if [ "${generate_csproj}" = "true" ]; then
    for example_dir in "${examples[@]}"
    do
      echo "Checking ${example_dir} project compiles"
      dotnet build "${example_dir}/"
    done
  fi
}

generated_files_cleanup && change_config
sqlc -f "${config_file}" generate

test_functions=("check_csproj_file" "check_project_compiles")
for test_function in "${test_functions[@]}"; do
  ${test_function}
  status_code=$?
  if [ ${status_code} -ne 0 ]; then
    echo "Function ${test_function} failed with status code ${status_code}"
    exit "${status_code}"
  fi
  echo "Test ${test_function} passed"
done