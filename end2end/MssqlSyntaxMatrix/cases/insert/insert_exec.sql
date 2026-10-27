-- @description INSERT ... EXEC from a stored procedure
-- @schema
CREATE TABLE author_archive (id BIGINT NOT NULL, name NVARCHAR(100) NOT NULL);
GO
CREATE PROCEDURE dbo.list_author_names AS SELECT id, name FROM authors;
GO
-- @queries
-- name: ArchiveAuthors :execrows
INSERT INTO author_archive (id, name) EXEC dbo.list_author_names;
