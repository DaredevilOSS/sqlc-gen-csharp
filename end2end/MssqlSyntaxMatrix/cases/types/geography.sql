-- @description `GEOGRAPHY` column: nullable and NOT NULL, read, written
-- @schema
CREATE TABLE t_geography (v GEOGRAPHY NULL, nn GEOGRAPHY NOT NULL);
-- @queries
-- name: SelectGeography :many
SELECT v, nn FROM t_geography;

-- name: InsertGeography :exec
INSERT INTO t_geography (v, nn) VALUES (@v, @nn);
