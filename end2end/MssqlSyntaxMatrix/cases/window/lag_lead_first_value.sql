-- @description LAG, LEAD, FIRST_VALUE and LAST_VALUE with a frame
-- @queries
-- name: PriceNeighbours :many
SELECT id,
       LAG(price) OVER (ORDER BY published_at) AS previous_price,
       LEAD(price, 1, 0) OVER (ORDER BY published_at) AS next_price,
       FIRST_VALUE(title) OVER (PARTITION BY author_id ORDER BY published_at) AS first_title,
       LAST_VALUE(title) OVER (PARTITION BY author_id ORDER BY published_at ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS last_title
FROM books;
