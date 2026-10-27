-- @description COALESCE, ISNULL and NULLIF: result types, nullability, and parameter inference
-- @queries
-- name: Fallbacks :many
SELECT id, COALESCE(bio, name) AS bio_or_name, ISNULL(bio, N'') AS bio_or_empty, NULLIF(name, N'') AS name_or_null FROM authors;

-- name: ByPattern :many
SELECT id, name FROM authors WHERE name LIKE COALESCE(@pattern, N'%');

-- name: ByPatternIsNull :many
SELECT id, name FROM authors WHERE name LIKE ISNULL(@pattern, N'%');
