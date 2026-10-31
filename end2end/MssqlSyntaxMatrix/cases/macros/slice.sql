-- @description sqlc.slice for integers, strings, NOT IN and several slices in one query
-- @queries
-- name: ByIds :many
SELECT id, name FROM authors WHERE id IN (sqlc.slice('ids'));

-- name: ByNames :many
SELECT id FROM authors WHERE name IN (sqlc.slice('names'));

-- name: NotByIds :many
SELECT id FROM authors WHERE id NOT IN (sqlc.slice('excluded'));

-- name: ByIdsAndTitles :many
SELECT id FROM books WHERE author_id IN (sqlc.slice('author_ids')) AND title IN (sqlc.slice('titles'));
