-- @description VALUES as a derived table
-- @queries
-- name: ListConstants :many
SELECT v.code, v.label FROM (VALUES (1, N'one'), (2, N'two')) AS v (code, label);
