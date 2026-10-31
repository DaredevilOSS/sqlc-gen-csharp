-- @description `:execlastid` via SELECT SCOPE_IDENTITY() after the INSERT
-- @queries
-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name) VALUES (@name);
SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
