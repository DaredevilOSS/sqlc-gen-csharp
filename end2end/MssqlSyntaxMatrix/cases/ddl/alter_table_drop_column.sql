-- @description ALTER TABLE ... DROP COLUMN
-- @schema
ALTER TABLE authors DROP COLUMN birth_date;
-- @queries
-- name: ListAuthors :many
SELECT * FROM authors;
