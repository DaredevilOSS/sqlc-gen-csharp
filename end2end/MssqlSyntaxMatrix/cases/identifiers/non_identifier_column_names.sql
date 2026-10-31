-- @description Column names that are not valid C# identifiers: leading digits, spaces, dashes, Unicode
-- @schema
CREATE TABLE odd_names ([1st place] INT NULL, [e-mail] NVARCHAR(50) NULL, [naïve] BIT NULL, [Total $] MONEY NULL);
-- @queries
-- name: ListOdd :many
SELECT [1st place], [e-mail], [naïve], [Total $] FROM odd_names;
