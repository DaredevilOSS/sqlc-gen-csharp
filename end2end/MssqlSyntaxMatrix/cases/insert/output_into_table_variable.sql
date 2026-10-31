-- @description OUTPUT ... INTO a table variable inside a multi-statement query
-- @queries
-- name: CreateAuthorOutputInto :one
DECLARE @ids TABLE (id BIGINT);
INSERT INTO authors (name) OUTPUT INSERTED.id INTO @ids VALUES (@name);
SELECT id FROM @ids;
