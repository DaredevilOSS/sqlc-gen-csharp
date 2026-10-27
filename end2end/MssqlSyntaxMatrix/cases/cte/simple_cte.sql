-- @description A single common table expression
-- @queries
-- name: CheapBooks :many
WITH cheap AS (SELECT id, title FROM books WHERE price < @max_price)
SELECT id, title FROM cheap;
