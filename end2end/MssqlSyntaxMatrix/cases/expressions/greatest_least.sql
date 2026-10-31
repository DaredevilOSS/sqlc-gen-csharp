-- @description GREATEST and LEAST (SQL Server 2022)
-- @queries
-- name: Clamp :many
SELECT id, GREATEST(price, @floor) AS at_least, LEAST(price, @ceiling) AS at_most FROM books;
