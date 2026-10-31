-- @description `FLOAT(24)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_float_24 (v FLOAT(24) NULL, nn FLOAT(24) NOT NULL);
-- @queries
-- name: SelectFloat24 :many
SELECT v, nn FROM t_float_24;

-- name: InsertFloat24 :exec
INSERT INTO t_float_24 (v, nn) VALUES (@v, @nn);

-- name: FilterFloat24 :many
SELECT v FROM t_float_24 WHERE nn = @nn;
