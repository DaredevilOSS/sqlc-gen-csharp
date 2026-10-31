-- @description Join across dbo-qualified and bracket-quoted names
-- @queries
-- name: QualifiedJoin :many
SELECT [a].[name], [b].[title] FROM [dbo].[authors] AS [a] JOIN dbo.books AS b ON b.author_id = [a].[id];
