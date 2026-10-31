-- @description FOR JSON PATH / AUTO producing a single JSON column
-- @queries
-- name: AuthorsAsJson :one
SELECT id, name FROM authors FOR JSON PATH;

-- name: AuthorAsJsonObject :one
SELECT id, name FROM authors WHERE id = @id FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER;
