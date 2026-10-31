-- @description The optional filter idiom: (@p IS NULL OR column = @p)
-- @queries
-- name: FilterBooks :many
SELECT id FROM books WHERE (@author_id IS NULL OR author_id = @author_id) AND (@title IS NULL OR title = @title);
