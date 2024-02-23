SHELL := /bin/bash
CURRENT_DIR := $(shell pwd)
PATH  := ${PATH}:${CURRENT_DIR}/dist

buf-gen:
	buf generate --template buf.gen.yaml buf.build/sqlc/sqlc --path plugin/

dotnet-publish:
	dotnet publish SqlcGenCsharp --runtime osx-arm64 -c release --output ./dist/

sqlc-generate:
	sqlc -f sqlc.local.yaml generate
