-- @description Aggregates over windows, including a running total
-- @queries
-- name: RunningTotals :many
SELECT id, SUM(price) OVER (PARTITION BY author_id ORDER BY id ROWS UNBOUNDED PRECEDING) AS running_total,
       COUNT(*) OVER () AS total_count
FROM books;
