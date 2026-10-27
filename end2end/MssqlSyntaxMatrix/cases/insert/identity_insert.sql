-- @description Explicit values for an IDENTITY column with SET IDENTITY_INSERT
-- @queries
-- name: CreateAuthorWithId :exec
SET IDENTITY_INSERT authors ON;
INSERT INTO authors (id, name) VALUES (@id, @name);
SET IDENTITY_INSERT authors OFF;
