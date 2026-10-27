-- @description CROSS JOIN and the comma join
-- @queries
-- name: CrossJoin :many
SELECT a.id AS author_id, b.id AS book_id FROM authors a CROSS JOIN books b;

-- name: CommaJoin :many
SELECT a.id AS author_id, b.id AS book_id FROM authors a, books b WHERE a.id = b.author_id;
