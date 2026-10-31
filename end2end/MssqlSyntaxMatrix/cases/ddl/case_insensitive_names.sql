-- @description Object names are case-insensitive under the default collation
-- @queries
-- name: ListAuthorsUpper :many
SELECT ID, NAME FROM AUTHORS WHERE NAME = @Name;
