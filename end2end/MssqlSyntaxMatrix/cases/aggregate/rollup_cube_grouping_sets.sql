-- @description ROLLUP, CUBE, GROUPING SETS and GROUPING()
-- @queries
-- name: Rollup :many
SELECT author_id, is_available, SUM(price) AS total, GROUPING(author_id) AS is_total FROM books GROUP BY ROLLUP (author_id, is_available);

-- name: Cube :many
SELECT author_id, is_available, COUNT(*) AS cnt FROM books GROUP BY CUBE (author_id, is_available);

-- name: GroupingSets :many
SELECT author_id, is_available, COUNT(*) AS cnt FROM books GROUP BY GROUPING SETS ((author_id), (is_available), ());
