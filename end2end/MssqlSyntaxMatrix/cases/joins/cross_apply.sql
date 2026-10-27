-- @description CROSS APPLY and OUTER APPLY with a correlated TOP subquery
-- @queries
-- name: LatestBookPerAuthor :many
SELECT a.id, latest.title FROM authors a
CROSS APPLY (SELECT TOP 1 b.title FROM books b WHERE b.author_id = a.id ORDER BY b.published_at DESC) AS latest;

-- name: LatestBookPerAuthorOuter :many
SELECT a.id, latest.title FROM authors a
OUTER APPLY (SELECT TOP 1 b.title FROM books b WHERE b.author_id = a.id ORDER BY b.published_at DESC) AS latest;
