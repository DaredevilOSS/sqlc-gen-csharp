-- @description Aliases quoted with brackets, double quotes and single quotes
-- @queries
-- name: ListQuotedAliases :many
SELECT id AS [Author Id], name AS "Author Name", bio 'Biography' FROM authors;
