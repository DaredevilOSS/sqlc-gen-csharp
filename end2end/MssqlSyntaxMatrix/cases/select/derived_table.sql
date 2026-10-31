-- @description Derived table (subquery in FROM) with column aliases
-- @queries
-- name: ListExpensive :many
SELECT t.book_id, t.price FROM (SELECT id AS book_id, price FROM books WHERE price > @min) AS t;
