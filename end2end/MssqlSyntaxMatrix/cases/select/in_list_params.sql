-- @description IN with a list of scalar parameters
-- @queries
-- name: ByThreeIds :many
SELECT id, name FROM authors WHERE id IN (@first_id, @second_id, @third_id);
