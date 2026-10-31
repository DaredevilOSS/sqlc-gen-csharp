-- @description Dynamic ORDER BY driven by a parameter
-- @queries
-- name: SortedAuthors :many
SELECT id, name FROM authors
ORDER BY CASE WHEN @sort = 'name' THEN name END, CASE WHEN @sort = 'id' THEN id END DESC;
