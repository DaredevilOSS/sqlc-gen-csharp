SHELL 		:= /bin/bash
PWD 		:= $(shell pwd)
PATH  		:= ${PATH}:${PWD}/dist
RUNTIME 	:= osx-arm64

buf-gen:
	buf generate --template buf.gen.yaml buf.build/sqlc/sqlc --path plugin/

dotnet-publish:
	dotnet publish SqlcGenCsharp --runtime ${RUNTIME} -c release --output ./dist/
	cp SqlcGenCsharp/bin/Release/net8.0/${RUNTIME}/SqlcGenCsharp.wasm ./dist/

sqlc-generate: dotnet-publish
	sqlc -f sqlc.local.yaml generate
