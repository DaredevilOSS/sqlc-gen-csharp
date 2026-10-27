-- @description EXISTS / NOT EXISTS with a parameter inside the subquery
-- @queries
-- name: ListAuthorsWithBooksAbove :many
SELECT id FROM authors a WHERE EXISTS (SELECT 1 FROM books b WHERE b.author_id = a.id AND b.price > @min_price);

-- name: ListAuthorsWithoutBooks :many
SELECT id FROM authors a WHERE NOT EXISTS (SELECT 1 FROM books b WHERE b.author_id = a.id);
