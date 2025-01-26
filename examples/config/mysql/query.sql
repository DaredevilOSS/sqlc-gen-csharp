-- name: GetAuthor :one
SELECT * FROM authors WHERE name = ? LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :exec
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: GetAuthorById :one
SELECT * FROM authors WHERE id = ? LIMIT 1;

-- name: DeleteAuthor :exec
DELETE FROM authors WHERE name = ?;

-- name: TruncateAuthors :exec
TRUNCATE TABLE authors;

-- name: UpdateAuthors :execrows
UPDATE authors
SET bio = sqlc.arg('bio')
WHERE bio IS NOT NULL;

-- name: SelectAuthorsWithSlice :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids'));

-- name: TruncateCopyToTests :exec
TRUNCATE TABLE copy_tests;

-- name: CopyToTests :copyfrom
INSERT INTO copy_tests (c_int, c_varchar, c_date, c_timestamp) VALUES (?, ?, ?, ?);

-- name: CountCopyRows :one
SELECT COUNT(1) AS cnt FROM copy_tests;

/* name: Test :one */
SELECT * FROM node_mysql_types LIMIT 1;