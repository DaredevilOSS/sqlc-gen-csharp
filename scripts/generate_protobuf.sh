#!/usr/bin/env bash

if wget -q --spider http://google.com; then
  buf generate --template buf.gen.yaml buf.build/sqlc/sqlc --path plugin/
else
  echo "No internet connection - using pre-existing protobuf files.."
fi