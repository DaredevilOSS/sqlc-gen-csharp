SHELL 		:= /bin/bash
PWD 		:= $(shell pwd)
        
dotnet-format:
	dotnet format \
		--exclude GeneratedProtobuf \
		--exclude MySqlConnectorExample \
		--exclude NpgsqlExample

dockerfile-generate:
	./scripts/generate_dockerfile.sh
        
protobuf-generate:
	./scripts/generate_protobuf.sh

# tests are run against generated code - can be generated either via a "process" or "wasm" SQLC plugins
run-tests:
	./scripts/run_tests.sh

# process type plugin
dotnet-build-process: protobuf-generate dotnet-format
	dotnet build SqlcGenCsharpProcess -c Release

dotnet-publish-process: dotnet-build-process
	dotnet publish SqlcGenCsharpProcess -c release --output dist/

sqlc-generate-process: dotnet-publish-process
	sqlc -f sqlc.process.yaml generate

test-process-plugin: sqlc-generate-process dockerfile-generate run-tests

# WASM type plugin
dotnet-publish-wasm: protobuf-generate
	dotnet publish SqlcGenCsharpWasm -c release --output dist/

update-wasm-plugin:
	./scripts/update_wasm_plugin.sh

sqlc-generate-wasm: dotnet-publish-wasm update-wasm-plugin
	SQLCCACHE=./; sqlc -f sqlc.wasm.yaml generate

test-wasm-plugin: sqlc-generate-wasm dockerfile-generate run-tests