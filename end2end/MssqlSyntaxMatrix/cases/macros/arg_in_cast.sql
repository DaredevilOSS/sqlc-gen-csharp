-- @description sqlc.arg inside CAST, which fixes its type
-- @queries
-- name: CastArg :many
SELECT id FROM books WHERE price > CAST(sqlc.arg('min_price') AS DECIMAL(10, 2));
