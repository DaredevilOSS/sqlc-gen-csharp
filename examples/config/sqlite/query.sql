-- name: GetAuthor :one
SELECT * FROM authors WHERE name = ? LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :exec
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES (?, ?) RETURNING id;

-- name: GetAuthorById :one
SELECT * FROM authors WHERE id = ? LIMIT 1;

-- name: UpdateAuthors :execrows
UPDATE authors 
SET bio = ?
WHERE bio IS NOT NULL;

-- name: DeleteAuthor :exec
DELETE FROM authors WHERE name = ?;

-- name: DeleteAllAuthors :exec
DELETE FROM authors;