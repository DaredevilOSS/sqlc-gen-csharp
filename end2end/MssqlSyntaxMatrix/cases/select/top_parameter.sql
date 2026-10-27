-- @description TOP with a parenthesized parameter
-- @queries
-- name: TopAuthors :many
SELECT TOP (@take) id, name FROM authors ORDER BY name;
