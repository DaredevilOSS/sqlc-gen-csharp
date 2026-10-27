-- @description Bitwise operators and bit columns
-- @queries
-- name: Bitwise :many
SELECT id, id & 1 AS low_bit, id | @mask AS masked, id ^ 2 AS xored, ~is_available AS not_available FROM books;
