-- @description `IMAGE` column: nullable and NOT NULL, read, written
-- @schema
CREATE TABLE t_image (v IMAGE NULL, nn IMAGE NOT NULL);
-- @queries
-- name: SelectImage :many
SELECT v, nn FROM t_image;

-- name: InsertImage :exec
INSERT INTO t_image (v, nn) VALUES (@v, @nn);
