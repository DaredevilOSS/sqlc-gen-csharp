-- @description Two result columns with the same name
-- @queries
-- name: DuplicateIds :many
SELECT a.id, b.id FROM authors a JOIN books b ON b.author_id = a.id;
