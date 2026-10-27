-- @description `GEOMETRY` column: nullable and NOT NULL, read, written
-- @schema
CREATE TABLE t_geometry (v GEOMETRY NULL, nn GEOMETRY NOT NULL);
-- @queries
-- name: SelectGeometry :many
SELECT v, nn FROM t_geometry;

-- name: InsertGeometry :exec
INSERT INTO t_geometry (v, nn) VALUES (@v, @nn);
