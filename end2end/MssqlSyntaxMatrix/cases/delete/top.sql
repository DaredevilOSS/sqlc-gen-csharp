-- @description DELETE TOP (@n)
-- @queries
-- name: DeleteSomeBooks :execrows
DELETE TOP (@n) FROM books WHERE is_available = 0;
