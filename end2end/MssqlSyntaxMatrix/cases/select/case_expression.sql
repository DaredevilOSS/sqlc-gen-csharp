-- @description Searched and simple CASE expressions
-- @queries
-- name: ListPriceBands :many
SELECT id,
       CASE WHEN price IS NULL THEN N'unknown' WHEN price < 10 THEN N'cheap' ELSE N'expensive' END AS band,
       CASE is_available WHEN 1 THEN 'yes' ELSE 'no' END AS available
FROM books;
