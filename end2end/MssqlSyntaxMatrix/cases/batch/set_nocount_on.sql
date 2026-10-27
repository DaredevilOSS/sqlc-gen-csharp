-- @description SET NOCOUNT ON in front of a query (a very common T-SQL habit)
-- @queries
-- name: ListAuthorsNoCount :many
SET NOCOUNT ON;
SELECT id, name FROM authors;
