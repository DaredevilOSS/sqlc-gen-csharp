-- @description Correlated scalar subquery in the select list
-- @queries
-- name: ListAuthorsWithBookCount :many
SELECT a.id, (SELECT COUNT(*) FROM books b WHERE b.author_id = a.id) AS book_count FROM authors a;
