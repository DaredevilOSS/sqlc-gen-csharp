-- @description UPDATE ... SET column = DEFAULT and = NULL
-- @queries
-- name: ResetBook :exec
UPDATE books SET is_available = DEFAULT, price = NULL WHERE id = @id;
