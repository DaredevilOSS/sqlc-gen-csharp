-- @description CREATE SYNONYM for a table
-- @schema
CREATE SYNONYM writers FOR dbo.authors;
-- @queries
-- name: ListWriters :many
SELECT id, name FROM writers;
