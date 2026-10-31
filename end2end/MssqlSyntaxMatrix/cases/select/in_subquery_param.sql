-- @description A parameter inside IN (subquery)
-- @queries
-- name: ListAuthorsOfTitle :many
SELECT id, name FROM authors WHERE id IN (SELECT author_id FROM books WHERE title = @title);
