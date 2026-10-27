-- @description SELECT without FROM: parameters and functions only
-- @queries
-- name: Now :one
SELECT SYSUTCDATETIME() AS now_utc;

-- name: AddOne :one
SELECT @value + 1 AS result;
