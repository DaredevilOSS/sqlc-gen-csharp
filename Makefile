SHELL 		:= /bin/bash
PWD 		:= $(shell pwd)
RUNTIME 	:= osx-arm64 # TODO automate
RUNTIME_DIR := SqlcGenCsharp/bin/Release/.net8.0/${RUNTIME}
PATH  		:= ${PATH}:${PWD}/${RUNTIME_DIR}

buf-gen:
	buf generate --template buf.gen.yaml buf.build/sqlc/sqlc --path plugin/

dotnet-publish:
	dotnet publish SqlcGenCsharp --runtime ${RUNTIME} -c release --output dist/

sqlc-generate: dotnet-publish
	sqlc -f sqlc.local.yaml generate
