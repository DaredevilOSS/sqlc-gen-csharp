-- @description TOP n WITH TIES and TOP n PERCENT
-- @queries
-- name: TopPricedWithTies :many
SELECT TOP 3 WITH TIES id, price FROM books ORDER BY price DESC;

-- name: TopTenPercent :many
SELECT TOP 10 PERCENT id FROM books ORDER BY id;
