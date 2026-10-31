-- @description N'' Unicode literals and string concatenation with +
-- @queries
-- name: Greetings :many
SELECT N'שלום ' + name AS greeting, 'plain' AS ascii FROM authors;
