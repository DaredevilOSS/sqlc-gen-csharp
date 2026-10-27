-- @description `TEXT` column: nullable and NOT NULL, read, written
-- @schema
CREATE TABLE t_text (v TEXT NULL, nn TEXT NOT NULL);
-- @queries
-- name: SelectText :many
SELECT v, nn FROM t_text;

-- name: InsertText :exec
INSERT INTO t_text (v, nn) VALUES (@v, @nn);
