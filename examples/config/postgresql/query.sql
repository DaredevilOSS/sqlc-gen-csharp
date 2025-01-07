-- name: GetAuthor :one
SELECT * FROM authors WHERE name = $1 LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :one
INSERT INTO authors (name, bio) VALUES ($1, $2) RETURNING *;

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES ($1, $2) RETURNING id;

-- name: GetAuthorById :one
SELECT * FROM authors WHERE id = $1 LIMIT 1;

-- name: DeleteAuthor :exec
DELETE FROM authors WHERE name = $1;

-- name: TruncateAuthors :exec
TRUNCATE TABLE authors;

-- name: UpdateAuthors :execrows
UPDATE authors 
   SET bio = $1
 WHERE bio IS NOT NULL;

-- name: TruncateCopyToTests :exec
TRUNCATE TABLE copy_tests;

-- name: TruncateNodePostgresTypes :exec
TRUNCATE TABLE node_postgres_types;

-- name: CopyToTests :copyfrom
INSERT INTO copy_tests (c_int, c_varchar, c_date, c_timestamp)
VALUES ($1, $2, $3, $4);

-- name: CountCopyRows :one
SELECT COUNT(1) AS cnt FROM copy_tests;

-- name: InsertNodePostgresType :execlastid
INSERT INTO node_postgres_types (c_smallint, c_boolean, c_integer, c_bigint, c_serial, c_decimal, c_numeric, c_real, c_double_precision, c_date, c_timestamp, c_char, c_varchar, c_character_varying, c_text, c_text_array)
VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15, $16) RETURNING id;

-- name: GetNodePostgresType :one
SELECT c_smallint, c_boolean, c_integer, c_bigint, c_serial, c_decimal, c_numeric, c_real, c_double_precision, c_date, c_timestamp, c_char, c_varchar, c_character_varying, c_text, c_text_array FROM node_postgres_types WHERE id = $1 LIMIT 1;