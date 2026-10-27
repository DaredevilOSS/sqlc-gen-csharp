-- @description A user-defined scalar function in the select list and WHERE clause
-- @schema
CREATE FUNCTION dbo.initials (@name NVARCHAR(100)) RETURNS NVARCHAR(10) AS
BEGIN
    RETURN LEFT(@name, 1);
END
GO
-- @queries
-- name: ListWithInitials :many
SELECT id, dbo.initials(name) AS initials FROM authors WHERE dbo.initials(name) = @initials;
