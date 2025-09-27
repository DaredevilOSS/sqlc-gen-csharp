SHELL 		:= /bin/bash
PWD 		:= $(shell pwd)

dotnet-build:
	dotnet build

.PHONY: unit-tests
unit-tests:
	dotnet test unit-tests/RepositoryTests
	sqlc generate -f sqlc.unit.test.yaml
	dotnet test unit-tests/CodegenTests

generate-end2end-tests:
	./end2end/scripts/generate_tests.sh
    
run-end2end-tests:
	./end2end/scripts/run_tests.sh

# process type plugin
dotnet-publish-process:
	dotnet publish LocalRunner -c release --output dist/

sync-sqlc-options:
	./scripts/sync_sqlc_options.sh

sqlc-generate-requests: dotnet-publish-process
	SQLCCACHE=./; sqlc -f sqlc.request.generated.yaml generate

sqlc-generate: sync-sqlc-options dotnet-publish-process sqlc-generate-requests
	SQLCCACHE=./; sqlc -f sqlc.local.generated.yaml generate

test-plugin: unit-tests sqlc-generate generate-end2end-tests dotnet-build run-end2end-tests

# WASM type plugin
setup-ci-wasm-plugin:
	dotnet publish WasmRunner -c release --output dist/
	./scripts/wasm/copy_plugin_to.sh dist
	./scripts/wasm/update_sha.sh sqlc.ci.yaml

# Manual
generate-protobuf:
	./scripts/generate_protobuf.sh