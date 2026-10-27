-- @description `:execrows` on UPDATE and DELETE
-- @queries
-- name: DisableAll :execrows
UPDATE books SET is_available = 0;

-- name: DeleteUnavailable :execrows
DELETE FROM books WHERE is_available = 0;
