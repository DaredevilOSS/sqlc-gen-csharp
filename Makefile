SHELL 		:= /bin/bash
PWD 		:= $(shell pwd)

RUNTIME_DIR := SqlcGenCsharp/bin/Release/.net8.0/osx-arm64 # TODO fix
PATH  		:= ${PATH}:${PWD}/${RUNTIME_DIR}

protobuf-generate:
	buf generate --template buf.gen.yaml buf.build/sqlc/sqlc --path plugin/

dotnet-build:
	dotnet build

dotnet-publish: protobuf-generate dotnet-build
	dotnet publish SqlcGenCsharp -c release --output dist/
	# cp ${RUNTIME_DIR}/SqlcGenCsharp.wasm dist/plugin.wasm

sqlc-generate: dotnet-publish
	sqlc -f sqlc.dev.yaml generate

run-tests: 
	./run_tests.sh

generate-and-test: sqlc-generate run-tests