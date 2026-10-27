-- @description Named WINDOW clause (SQL Server 2022), PERCENTILE_CONT and PERCENTILE_DISC
-- @queries
-- name: NamedWindow :many
SELECT id, SUM(price) OVER w AS total, AVG(price) OVER w AS average FROM books WINDOW w AS (PARTITION BY author_id);

-- name: Percentiles :many
SELECT DISTINCT author_id,
       PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY price) OVER (PARTITION BY author_id) AS median,
       PERCENTILE_DISC(0.5) WITHIN GROUP (ORDER BY price) OVER (PARTITION BY author_id) AS median_disc
FROM books;
