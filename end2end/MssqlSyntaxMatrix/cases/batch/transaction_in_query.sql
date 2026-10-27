-- @description Explicit BEGIN TRAN / COMMIT inside a query
-- @queries
-- name: MoveBooks :exec
BEGIN TRANSACTION;
UPDATE books SET author_id = @to_author_id WHERE author_id = @from_author_id;
DELETE FROM authors WHERE id = @from_author_id;
COMMIT TRANSACTION;
