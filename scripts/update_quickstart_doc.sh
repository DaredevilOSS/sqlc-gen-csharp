#!/usr/bin/env bash

set -e

doc_file=$1

sed -i 's/{PLUGIN_VERSION}/new-text/g' input.txt
sed -i 's/old-text/new-text/g' input.txt