-- @description Several CTEs referencing each other
-- @queries
-- name: AuthorTotals :many
WITH totals AS (SELECT author_id, SUM(price) AS total FROM books GROUP BY author_id),
     ranked AS (SELECT author_id, total, RANK() OVER (ORDER BY total DESC) AS rnk FROM totals)
SELECT a.name, r.total, r.rnk FROM ranked r JOIN authors a ON a.id = r.author_id;
