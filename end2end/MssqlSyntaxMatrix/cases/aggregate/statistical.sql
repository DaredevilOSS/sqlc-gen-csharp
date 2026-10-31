-- @description STDEV, VAR and APPROX_COUNT_DISTINCT
-- @queries
-- name: PriceStatistics :one
SELECT STDEV(price) AS stdev, VAR(price) AS variance, APPROX_COUNT_DISTINCT(author_id) AS approx_authors FROM books;
