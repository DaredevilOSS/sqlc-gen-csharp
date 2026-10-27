-- @description Calling a stored procedure with EXEC and named arguments
-- @schema
CREATE PROCEDURE dbo.get_author @id BIGINT AS
BEGIN
    SELECT id, name FROM authors WHERE id = @id;
END
GO
-- @queries
-- name: GetAuthorViaProc :many
EXEC dbo.get_author @id = @id;
