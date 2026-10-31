-- @description Arithmetic operators, integer division, modulo and type promotion
-- @queries
-- name: Arithmetic :many
SELECT id, price * 2 AS doubled, id / 2 AS half_id, id % 3 AS remainder, price + @delta AS adjusted, -id AS negated FROM books;
