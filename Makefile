SHELL 		:= /bin/bash
PWD 		:= $(shell pwd)

 # TODO automate these 2
RUNTIME 	:= osx-arm64
RUNTIME_DIR := SqlcGenCsharp/bin/Release/net8.0/${RUNTIME}

PATH  		:= ${PATH}:${PWD}/${RUNTIME_DIR}

protobuf-generate:
	buf generate --template buf.gen.yaml buf.build/sqlc/sqlc --path plugin/

dotnet-build:
	dotnet build

dotnet-publish: protobuf-generate dotnet-build
	dotnet publish SqlcGenCsharp --runtime ${RUNTIME} -c release --output dist/
	# cp ${RUNTIME_DIR}/SqlcGenCsharp.wasm dist/plugin.wasm

sqlc-generate: dotnet-publish
	sqlc -f sqlc.dev.yaml generate

test-setup: test-teardown
	docker-compose up --wait -d
	docker exec -it mysqldb /bin/bash -c "mysql -h localhost --database tests < /var/db/schema.sql"

just-dotnet-test:
	dotnet test

dotnet-test: test-setup just-dotnet-test test-teardown

test-teardown:
	docker-compose down

generate-and-test: sqlc-generate dotnet-test