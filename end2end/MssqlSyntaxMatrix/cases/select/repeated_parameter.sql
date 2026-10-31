-- @description The same parameter used several times, with different case
-- @queries
-- name: ByNameTwice :many
SELECT id FROM authors WHERE name = @name OR bio = @Name OR @NAME IS NULL;
