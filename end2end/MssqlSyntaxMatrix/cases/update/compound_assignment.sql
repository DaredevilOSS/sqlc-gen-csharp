-- @description Compound assignment operators += and *=
-- @queries
-- name: AdjustPrice :exec
UPDATE books SET price += @delta, title += @suffix WHERE author_id = @author_id;
