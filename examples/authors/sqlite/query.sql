-- name: GetAuthor :one
SELECT * FROM authors WHERE name = ? LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :exec
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: DeleteAuthor :exec
DELETE FROM authors WHERE name = ?;

-- name: DeleteAllAuthors :exec
DELETE FROM authors;