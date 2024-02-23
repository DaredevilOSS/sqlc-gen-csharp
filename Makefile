buf-gen: buf.gen.yaml
	buf generate --template buf.gen.yaml buf.build/sqlc/sqlc --path plugin/