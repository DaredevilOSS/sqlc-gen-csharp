-- @description OFFSET ... ROWS FETCH NEXT ... ROWS ONLY with parameters
-- @queries
-- name: PageAuthors :many
SELECT id, name FROM authors ORDER BY name OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
