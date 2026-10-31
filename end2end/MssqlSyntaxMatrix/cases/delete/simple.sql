-- @description DELETE with a parameter
-- @queries
-- name: DeleteAuthor :exec
DELETE FROM authors WHERE id = @id;

-- name: DeleteAuthorNoFrom :exec
DELETE authors WHERE id = @id;
