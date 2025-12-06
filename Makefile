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
# Source files that should trigger a rebuild
WASM_SOURCES := $(shell find WasmRunner -name '*.cs' -o -name '*.csproj' 2>/dev/null | grep -v '/bin/' | grep -v '/obj/') \
                $(shell find SqlcGenCsharp -name '*.cs' -o -name '*.csproj' 2>/dev/null | grep -v '/bin/' | grep -v '/obj/' || true)

# Final output file - this is what we check for caching
WASM_PLUGIN_OUTPUT := dist/plugin.wasm

# Make setup-wasm-plugin depend on the actual output file (not phony)
setup-wasm-plugin: $(WASM_PLUGIN_OUTPUT)
	@echo "WASM plugin is up to date"

# Build and copy plugin if output doesn't exist or sources are newer
$(WASM_PLUGIN_OUTPUT): $(WASM_SOURCES)
	dotnet publish WasmRunner -c release --output dist/
	./scripts/wasm/copy_plugin_to.sh dist
	./scripts/wasm/update_sha.sh sqlc.ci.yaml

run-benchmark: sqlc-generate
	./benchmark/scripts/run_benchmark.sh

run-end2end-tests:
	./end2end/scripts/run_tests.sh
	
# Manual
generate-protobuf:
	./scripts/generate_protobuf.sh

dotnet-format:
	dotnet format --exclude GeneratedProtobuf --exclude examples

