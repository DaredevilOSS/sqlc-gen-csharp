-- @description FOR XML PATH, including the string-concatenation idiom
-- @queries
-- name: AuthorsAsXml :one
SELECT id, name FROM authors FOR XML PATH('author'), ROOT('authors');

-- name: ConcatenatedTitles :many
SELECT a.id, STUFF((SELECT N',' + b.title FROM books b WHERE b.author_id = a.id FOR XML PATH('')), 1, 1, N'') AS titles FROM authors a;
