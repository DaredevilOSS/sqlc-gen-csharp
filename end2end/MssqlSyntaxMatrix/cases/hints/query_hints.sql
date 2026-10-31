-- @description OPTION (RECOMPILE), OPTION (MAXDOP 1), OPTION (OPTIMIZE FOR UNKNOWN)
-- @queries
-- name: Recompiled :many
SELECT id FROM books WHERE author_id = @author_id OPTION (RECOMPILE);

-- name: SingleThreaded :many
SELECT id FROM books WHERE price > @min_price OPTION (MAXDOP 1, OPTIMIZE FOR UNKNOWN);
