-- @description LIKE with a parameter and an ESCAPE clause
-- @queries
-- name: SearchTitles :many
SELECT id, title FROM books WHERE title LIKE @pattern ESCAPE '\';
