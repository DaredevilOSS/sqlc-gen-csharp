#!/usr/bin/env bash

declare -a examples=("MySqlConnectorExample" "NpgsqlExample")

remove_generated_files() {
    generate_csproj=$1
    for example_dir in "${examples[@]}"
    do
        echo "Deleting .cs files" && rm "${example_dir}/*.cs"
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
      if [ "${file_per_query}" = "true" ]; then
          if [ "${file_count}" -gt 2 ]; then
              echo "Assertion passed: More than 2 .cs files are present in the directory ${example_dir}."
              return 0
          else
              echo "Assertion failed: Not more than 2 .cs files in the directory ${example_dir}."
              return 1
          fi
      else
          if [ "${file_count}" -eq 2 ]; then
              echo "Assertion passed: Exactly 2 .cs files are present in the directory ${example_dir}."
              return 0
          else
              echo "Assertion failed: Not exactly 2 .cs files in the directory ${example_dir}."
              return 1
          fi
      fi
    done
}

check_csproj_file() {
    generate_csproj=$1
    for example_dir in "${examples[@]}"
    do
        if [ -f "${example_dir}/${example_dir}.csproj" ]; then
            echo "Assertion failed: A .csproj file is present in the directory ${example_dir}."
            return 1
        fi
    done
}

file_per_query=$1
generate_csproj=$2

remove_generated_files "${generate_csproj}"
sqlc -f sqlc.ci.yaml generate

status_code=$(check_cs_file_count "${file_per_query}")
if [ "${status_code}" -ne 0 ]; then
    exit 1
fi
status_code=$(check_csproj_file "${generate_csproj}")
if [ "${status_code}" -ne 0 ]; then
    exit 1
fi