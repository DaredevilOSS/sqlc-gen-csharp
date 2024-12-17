SHELL 		:= /bin/bash
PWD 		:= $(shell pwd)
        
dotnet-format:
	dotnet format \
		--exclude GeneratedProtobuf \
		--exclude MySqlConnectorExample \
		--exclude NpgsqlExample \
		--exclude NpgsqlDapperExample \
		--exclude SqliteExample

dockerfile-generate:
	./scripts/generate_dockerfile.sh Dockerfile
        
protobuf-generate:
	#./scripts/generate_protobuf.sh

run-end2end-tests:
	./scripts/tests/run_end2end.sh

# process type plugin
dotnet-build-process: protobuf-generate dotnet-format
	dotnet build LocalRunner -c Release

dotnet-publish-process: dotnet-build-process
	dotnet publish LocalRunner -c release --output dist/

run-codegen-tests-process:
	./scripts/tests/run_codegen_matrix.sh sqlc.local.yaml

sqlc-generate-process: dotnet-publish-process
	sqlc -f sqlc.local.yaml generate

test-process-plugin: sqlc-generate-process dockerfile-generate run-end2end-tests

# WASM type plugin
dotnet-publish-wasm: protobuf-generate
	dotnet publish WasmRunner -c release --output dist/
	./scripts/wasm/copy_plugin_to.sh dist

update-wasm-plugin:
	./scripts/wasm/update_sha.sh sqlc.ci.yaml

sqlc-generate-wasm: dotnet-publish-wasm update-wasm-plugin
	SQLCCACHE=./; sqlc -f sqlc.ci.yaml generate

run-codegen-tests-wasm:
	./scripts/tests/run_codegen_matrix.sh sqlc.ci.yaml

test-wasm-plugin: sqlc-generate-wasm update-wasm-plugin dockerfile-generate run-end2end-tests