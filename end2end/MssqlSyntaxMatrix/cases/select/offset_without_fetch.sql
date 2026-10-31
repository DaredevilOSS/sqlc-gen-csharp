-- @description OFFSET ... ROWS without FETCH
-- @queries
-- name: SkipAuthors :many
SELECT id FROM authors ORDER BY id OFFSET @skip ROWS;
