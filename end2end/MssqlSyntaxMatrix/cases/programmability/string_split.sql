-- @description STRING_SPLIT as a table-valued function fed by a parameter
-- @queries
-- name: AuthorsByNameList :many
SELECT id, name FROM authors WHERE name IN (SELECT value FROM STRING_SPLIT(@names, ','));
