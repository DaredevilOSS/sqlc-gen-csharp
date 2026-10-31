-- @description T-SQL column alias written as alias = expression
-- @queries
-- name: ListAuthorNames :many
SELECT author_id = id, author_name = name FROM authors;
