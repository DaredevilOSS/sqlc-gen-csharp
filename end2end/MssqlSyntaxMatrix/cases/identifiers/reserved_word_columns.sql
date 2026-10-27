-- @description Columns named after T-SQL and C# keywords
-- @schema
CREATE TABLE keywords ([user] NVARCHAR(20) NOT NULL, [order] INT NOT NULL, [key] INT NULL, [class] INT NULL, [string] NVARCHAR(10) NULL);
-- @queries
-- name: ListKeywords :many
SELECT [user], [order], [key], [class], [string] FROM keywords WHERE [user] = @user;
