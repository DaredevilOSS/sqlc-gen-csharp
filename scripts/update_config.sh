#!/usr/bin/env bash

set -ex

file_per_query=$1
yq -i ".sql[0].codegen[0].options.filePerQuery = ${file_per_query}" sqlc.wasm.yaml
yq -i ".sql[1].codegen[0].options.filePerQuery = ${file_per_query}" sqlc.wasm.yaml