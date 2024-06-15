#!/usr/bin/env bash

set -e

declare -a examples=("MySqlConnectorExample" "NpgsqlExample")

generated_files_cleanup() {
    generate_csproj=$1
    for example_dir in "${examples[@]}"
    do
        echo "Deleting .cs files" && find "${example_dir}/" -type f -name "*.cs" -exec rm -f {} \;
        if [ "${generate_csproj}" = "true" ]; then
            echo "Deleting .csproj file" && rm "${example_dir}/${example_dir}.csproj"
        fi
    done
}

check_cs_file_count() {
    file_per_query=$1
    for example_dir in "${examples[@]}"
    do
      file_count=$(find "${example_dir}/" -maxdepth 1 -name "*.cs" 2>/dev/null | wc -l)
      if [[ "${file_per_query}" = "true" && "${file_count}" -le 2 ]]; then
          echo "Assertion failed: Not more than 2 .cs files in the directory ${example_dir}."
          return 1
      elif [[ "${file_per_query}" = "false" && "${file_count}" -ne 2 ]]; then
          echo "Assertion failed: Not exactly 2 .cs files in the directory ${example_dir}."
          return 1
      fi
    done
}

check_csproj_file() {
    for example_dir in "${examples[@]}"
    do
      if [ ! -f "${example_dir}/${example_dir}.csproj" ]; then
          echo "Assertion failed: A .csproj file is not present in the directory ${example_dir}."
          return 1
      fi
    done
}

check_project_compiles() {
    for example_dir in "${examples[@]}"
    do
      dotnet build "${example_dir}/"
    done
}

config_file=$1
file_per_query=$2
generate_csproj=$3
target_framework=$4

yq -i "
  .sql[0].codegen[0].options.filePerQuery = ${file_per_query} |
  .sql[1].codegen[0].options.filePerQuery = ${file_per_query} |
  .sql[0].codegen[0].options.generateCsproj = ${generate_csproj} |
  .sql[1].codegen[0].options.generateCsproj = ${generate_csproj} |
  .sql[0].codegen[0].options.targetFramework = \"${target_framework}\" |
  .sql[1].codegen[0].options.targetFramework = \"${target_framework}\"
" "${config_file}"

generated_files_cleanup "${generate_csproj}"
echo "Using the following codegen config:" && \
  yq '.sql[0].codegen[0]' "${config_file}" && \
  yq '.sql[1].codegen[0]' "${config_file}"
sqlc -f "${config_file}" generate

status_code=$(check_cs_file_count "${file_per_query}")
if [ "${status_code}" -ne 0 ]; then
    exit "${status_code}"
fi
status_code=$(check_csproj_file)
if [ "${status_code}" -ne 0 ]; then
    exit "${status_code}"
fi
if [ "${generate_csproj}" = "true" ]; then
    status_code=$(check_project_compiles "${target_framework}")
    if [ "${status_code}" -ne 0 ]; then
        exit "${status_code}"
    fi
fi