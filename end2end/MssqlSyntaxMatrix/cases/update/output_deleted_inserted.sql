-- @description UPDATE ... OUTPUT DELETED.x, INSERTED.x
-- @queries
-- name: RenameAuthor :one
UPDATE authors SET name = @name OUTPUT DELETED.name AS old_name, INSERTED.name AS new_name WHERE id = @id;
