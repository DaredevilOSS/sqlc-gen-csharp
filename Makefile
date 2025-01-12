SHELL 		:= /bin/bash
PWD 		:= $(shell pwd)
        
dotnet-format:
	dotnet format --exclude GeneratedProtobuf --exclude examples
        
protobuf-generate:
	./scripts/generate_protobuf.sh

unit-tests:
	dotnet test SqlcGenCsharpTests
    
run-end2end-tests:
	./end2end/scripts/run_tests.sh

# process type plugin
dotnet-build-process: protobuf-generate dotnet-format
	dotnet build LocalRunner -c Release

dotnet-publish-process: dotnet-build-process
	dotnet publish LocalRunner -c release --output dist/

sqlc-generate-process: dotnet-publish-process
	sqlc -f sqlc.local.yaml generate

test-process-plugin: unit-tests sqlc-generate-process run-end2end-tests

# WASM type plugin
dotnet-publish-wasm: protobuf-generate
	dotnet publish WasmRunner -c release --output dist/
	./scripts/wasm/copy_plugin_to.sh dist

update-wasm-plugin:
	./scripts/wasm/update_sha.sh sqlc.ci.yaml

sqlc-generate-wasm: dotnet-publish-wasm update-wasm-plugin
	SQLCCACHE=./; sqlc -f sqlc.ci.yaml generate

test-wasm-plugin: unit-tests sqlc-generate-wasm update-wasm-plugin run-end2end-tests