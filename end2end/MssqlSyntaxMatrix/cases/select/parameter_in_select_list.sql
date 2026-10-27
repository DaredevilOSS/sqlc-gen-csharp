-- @description A parameter echoed in the select list, with and without CAST
-- @queries
-- name: EchoParams :many
SELECT id, @label AS label, CAST(@factor AS DECIMAL(5, 2)) * price AS scaled FROM books;
