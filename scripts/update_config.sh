#!/usr/bin/env bash

set -ex

filename=$1
file_per_query=$2
yq -i "
  .sql[0].codegen[0].options.filePerQuery = ${file_per_query} |
  .sql[1].codegen[0].options.filePerQuery = ${file_per_query}
" "$filename"
