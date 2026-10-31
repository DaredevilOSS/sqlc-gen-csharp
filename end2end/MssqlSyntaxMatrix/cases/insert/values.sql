-- @description INSERT ... VALUES with parameters for every column
-- @queries
-- name: CreateAuthor :exec
INSERT INTO authors (name, bio, birth_date) VALUES (@name, @bio, @birth_date);
