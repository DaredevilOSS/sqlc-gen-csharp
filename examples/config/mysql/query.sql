-- name: GetAuthor :one
SELECT * FROM authors WHERE name = ? LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :exec
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: UpdateAuthor :exec
UPDATE authors SET bio = ? WHERE id = ?;

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: DeleteAuthor :exec
DELETE FROM authors WHERE name = ?;

-- name: TruncateAuthors :exec
TRUNCATE TABLE authors;

-- name: UpdateAuthors :execrows
UPDATE authors
SET bio = sqlc.arg('bio')
WHERE bio IS NOT NULL;

/* name: Test :one */
SELECT * FROM node_mysql_types LIMIT 1;