#!/usr/bin/env bash

set -ex

filename=$1
file_per_query=$2
generate_csproj=$3
yq -i "
  .sql[0].codegen[0].options.filePerQuery = ${file_per_query} |
  .sql[1].codegen[0].options.filePerQuery = ${file_per_query} |
  .sql[0].codegen[0].options.generateCsproj = ${generate_csproj} |
  .sql[1].codegen[0].options.generateCsproj = ${generate_csproj}
" "$filename"
