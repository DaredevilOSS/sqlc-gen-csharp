-- @description An inline table-valued function in FROM
-- @schema
CREATE FUNCTION dbo.books_of (@author_id BIGINT) RETURNS TABLE AS
RETURN (SELECT id, title FROM books WHERE author_id = @author_id);
GO
-- @queries
-- name: BooksOf :many
SELECT id, title FROM dbo.books_of(@author_id);
