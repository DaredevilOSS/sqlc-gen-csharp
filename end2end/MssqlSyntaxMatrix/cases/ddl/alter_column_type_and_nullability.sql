-- @description ALTER TABLE ... ALTER COLUMN changes the type and nullability
-- @schema
ALTER TABLE authors ALTER COLUMN bio NVARCHAR(500) NOT NULL;
-- @queries
-- name: ListBios :many
SELECT id, bio FROM authors;
