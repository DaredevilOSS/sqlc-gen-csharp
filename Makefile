SHELL 		:= /bin/bash
PWD 		:= $(shell pwd)

# TODO automate
WASM_FILE	:= SqlcGenCsharpWasm/bin/Release/net8.0/wasi-wasm/AppBundle/dotnet.wasm
RUNTIME_DIR := SqlcGenCsharp/bin/Release/.net8.0/osx-arm64/
PATH  		:= ${PATH}:${PWD}/${RUNTIME_DIR}

protobuf-generate:
	buf generate --template buf.gen.yaml buf.build/sqlc/sqlc --path plugin/

# tests are run against generated code - can be generated either via a "process" or "wasm" SQLC plugins
run-tests:
	./run_tests.sh

# process type plugin
dotnet-build-process:
	dotnet build SqlcGenCsharpProcess -c Release

dotnet-publish-process: protobuf-generate dotnet-build-process
	dotnet publish SqlcGenCsharpProcess -c release --output dist/

sqlc-generate-process: dotnet-publish-process
	sqlc -f sqlc.process.yaml generate

test-process-plugin: sqlc-generate-process run-tests

# WASM type plugin
dotnet-build-wasm:
	dotnet build SqlcGenCsharpWasm -c Release

dotnet-publish-wasm: dotnet-build-wasm
	dotnet publish SqlcGenCsharpWasm -c release --output dist/ && cp ${WASM_FILE} dist/plugin.wasm

sqlc-generate-from-wasm: dotnet-publish-wasm
	sqlc -f sqlc.wasm.yaml generate

test-wasm-plugin: sqlc-generate-from-wasm run-tests