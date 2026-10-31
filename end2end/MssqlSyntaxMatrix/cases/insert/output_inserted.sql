-- @description INSERT ... OUTPUT INSERTED.* / INSERTED.col returning rows
-- @queries
-- name: CreateAuthorReturningAll :one
INSERT INTO authors (name, bio) OUTPUT INSERTED.* VALUES (@name, @bio);

-- name: CreateAuthorReturningSome :one
INSERT INTO authors (name) OUTPUT INSERTED.id, INSERTED.name VALUES (@name);
