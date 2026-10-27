-- @description IIF and CHOOSE
-- @queries
-- name: ListFlags :many
SELECT id, IIF(price > @threshold, 1, 0) AS is_expensive, CHOOSE(2, 'a', 'b', 'c') AS second FROM books;
