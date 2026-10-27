-- @description UPDATE TOP (@n)
-- @queries
-- name: MarkSomeUnavailable :execrows
UPDATE TOP (@n) books SET is_available = 0 WHERE is_available = 1;
