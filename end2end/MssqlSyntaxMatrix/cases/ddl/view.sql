-- @description Querying a VIEW
-- @schema
CREATE VIEW author_books AS
SELECT a.name AS author_name, b.title
FROM authors a JOIN books b ON b.author_id = a.id;
-- @queries
-- name: ListAuthorBooks :many
SELECT author_name, title FROM author_books WHERE author_name = @author_name;
