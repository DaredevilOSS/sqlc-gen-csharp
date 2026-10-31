-- @description Parameter names with digits, underscores and mixed case
-- @queries
-- name: OddParams :many
SELECT id FROM authors WHERE id = @Id1 OR name = @first_Name OR bio = @_bio;
