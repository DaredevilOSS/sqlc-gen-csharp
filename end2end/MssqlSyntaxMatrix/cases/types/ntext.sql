-- @description `NTEXT` column: nullable and NOT NULL, read, written
-- @schema
CREATE TABLE t_ntext (v NTEXT NULL, nn NTEXT NOT NULL);
-- @queries
-- name: SelectNtext :many
SELECT v, nn FROM t_ntext;

-- name: InsertNtext :exec
INSERT INTO t_ntext (v, nn) VALUES (@v, @nn);
