-- @description Self join with aliases
-- @queries
-- name: SameNameAuthors :many
SELECT a1.id AS first_id, a2.id AS second_id FROM authors a1 JOIN authors a2 ON a1.name = a2.name AND a1.id < a2.id;
