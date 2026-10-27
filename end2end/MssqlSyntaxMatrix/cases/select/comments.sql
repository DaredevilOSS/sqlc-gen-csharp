-- @description Block comments, nested block comments and trailing line comments inside a query
-- @queries
-- name: Commented :many
SELECT id, /* the name */ name -- trailing
FROM authors /* outer /* nested */ still a comment */
WHERE id > @min_id;
