-- @description `REAL` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_real (v REAL NULL, nn REAL NOT NULL);
-- @queries
-- name: SelectReal :many
SELECT v, nn FROM t_real;

-- name: InsertReal :exec
INSERT INTO t_real (v, nn) VALUES (@v, @nn);

-- name: FilterReal :many
SELECT v FROM t_real WHERE nn = @nn;
