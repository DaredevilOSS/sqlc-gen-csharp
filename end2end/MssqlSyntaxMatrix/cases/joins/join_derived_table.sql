-- @description Join to an aggregated derived table
-- @queries
-- name: AuthorsWithTotals :many
SELECT a.name, t.total FROM authors a
JOIN (SELECT author_id, SUM(price) AS total FROM books GROUP BY author_id) t ON t.author_id = a.id;
