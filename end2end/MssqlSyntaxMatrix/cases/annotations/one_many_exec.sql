-- @description `:one`, `:many`, `:exec` basics
-- @queries
-- name: GetAuthor :one
SELECT id, name, bio FROM authors WHERE id = @id;

-- name: ListAuthors :many
SELECT id, name, bio FROM authors ORDER BY name;

-- name: DeleteAllAuthors :exec
DELETE FROM authors;
