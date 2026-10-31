-- @description sqlc.arg mixed with plain @params, sharing a name
-- @queries
-- name: Mixed :many
SELECT id FROM authors WHERE name = sqlc.arg('name') AND (bio = @bio OR bio = @name);
