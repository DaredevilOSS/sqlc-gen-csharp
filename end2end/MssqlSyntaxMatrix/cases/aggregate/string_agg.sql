-- @description STRING_AGG with WITHIN GROUP
-- @queries
-- name: TitlesPerAuthor :many
SELECT author_id, STRING_AGG(title, N', ') WITHIN GROUP (ORDER BY title) AS titles FROM books GROUP BY author_id;
