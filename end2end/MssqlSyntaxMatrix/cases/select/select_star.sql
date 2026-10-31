-- @description SELECT * and qualified t.* expansion
-- @queries
-- name: ListAuthors :many
SELECT * FROM authors;

-- name: ListAuthorsWithBookTitle :many
SELECT a.*, b.title FROM authors a JOIN books b ON b.author_id = a.id;
