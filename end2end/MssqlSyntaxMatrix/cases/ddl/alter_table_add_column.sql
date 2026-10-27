-- @description ALTER TABLE ... ADD column
-- @schema
ALTER TABLE authors ADD nickname NVARCHAR(50) NULL;
-- @queries
-- name: ListNicknames :many
SELECT id, nickname FROM authors;
