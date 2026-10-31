-- @description A multi-statement table-valued function in FROM
-- @schema
CREATE FUNCTION dbo.range_of (@upto INT) RETURNS @result TABLE (n INT NOT NULL) AS
BEGIN
    DECLARE @i INT = 1;
    WHILE @i <= @upto BEGIN INSERT INTO @result VALUES (@i); SET @i += 1; END;
    RETURN;
END
GO
-- @queries
-- name: Range :many
SELECT n FROM dbo.range_of(@upto);
