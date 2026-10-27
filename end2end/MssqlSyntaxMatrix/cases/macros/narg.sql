-- @description sqlc.narg makes a parameter nullable
-- @queries
-- name: NargFilter :many
SELECT id FROM authors WHERE name = sqlc.narg('name') OR sqlc.narg('name') IS NULL;
