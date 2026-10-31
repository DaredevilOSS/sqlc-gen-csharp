-- @description Column renamed with sp_rename (how T-SQL renames columns)
-- @schema
EXEC sp_rename 'authors.bio', 'biography', 'COLUMN';
-- @queries
-- name: ListBiographies :many
SELECT id, biography FROM authors;
