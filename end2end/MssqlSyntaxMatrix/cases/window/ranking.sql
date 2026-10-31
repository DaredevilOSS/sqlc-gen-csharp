-- @description ROW_NUMBER, RANK, DENSE_RANK and NTILE
-- @queries
-- name: RankedBooks :many
SELECT id,
       ROW_NUMBER() OVER (PARTITION BY author_id ORDER BY price DESC) AS rn,
       RANK() OVER (ORDER BY price DESC) AS rnk,
       DENSE_RANK() OVER (ORDER BY price DESC) AS dense_rnk,
       NTILE(4) OVER (ORDER BY price) AS quartile
FROM books;
