-- @description Parameters wrapped in functions and compared to columns
-- @queries
-- name: CaseInsensitiveMatch :many
SELECT id FROM authors WHERE LOWER(name) = LOWER(@name) AND LEN(@name) > 0;
