-- @description sqlc.arg in TOP and in OFFSET / FETCH
-- @queries
-- name: TopArg :many
SELECT TOP (sqlc.arg('take')) id FROM authors ORDER BY id;

-- name: PageArgs :many
SELECT id FROM authors ORDER BY id OFFSET sqlc.arg('skip') ROWS FETCH NEXT sqlc.arg('take') ROWS ONLY;
