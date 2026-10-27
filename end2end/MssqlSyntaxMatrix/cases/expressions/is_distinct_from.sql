-- @description IS [NOT] DISTINCT FROM (SQL Server 2022)
-- @queries
-- name: ChangedBios :many
SELECT id FROM authors WHERE bio IS DISTINCT FROM @bio;
