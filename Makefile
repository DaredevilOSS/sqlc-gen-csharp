SHELL 		:= /bin/bash
PWD 		:= $(shell pwd)
        
dotnet-format:
	dotnet format --exclude GeneratedProtobuf --exclude examples
        
protobuf-generate:
	./scripts/generate_protobuf.sh

dotnet-build:
	dotnet build

unit-tests:
	dotnet test RepositoryTests
	sqlc generate -f sqlc.unit.test.yaml
	dotnet test CodegenTests

generate-end2end-tests:
	./end2end/scripts/generate_tests.sh
    
run-end2end-tests: generate-end2end-tests
	./end2end/scripts/run_tests.sh

# process type plugin
dotnet-build-process: protobuf-generate dotnet-format
	dotnet build LocalRunner -c Release

dotnet-publish-process: dotnet-build-process
	dotnet publish LocalRunner -c release --output dist/

sqlc-generate-process: dotnet-publish-process
	sqlc -f sqlc.local.yaml generate

# WASM type plugin
dotnet-publish-wasm: protobuf-generate
	dotnet publish WasmRunner -c release --output dist/
	./scripts/wasm/copy_plugin_to.sh dist

update-wasm-plugin:
	./scripts/wasm/update_sha.sh sqlc.ci.yaml

sqlc-generate-wasm: dotnet-publish-wasm update-wasm-plugin
	SQLCCACHE=./; sqlc -f sqlc.ci.yaml generate

test-wasm-plugin: sqlc-generate-process unit-tests sqlc-generate-wasm update-wasm-plugin dotnet-build run-end2end-tests