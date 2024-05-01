#!/usr/bin/env bash

set -e

wget -q --spider http://google.com
if [ $? -eq 0 ]; then
  buf generate --template buf.gen.yaml buf.build/sqlc/sqlc --path plugin/
else
  echo "No internet connection - using pre-existing protobuf files.."
fi