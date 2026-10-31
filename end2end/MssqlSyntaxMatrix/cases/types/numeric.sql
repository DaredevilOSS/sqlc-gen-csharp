-- @description `NUMERIC(10, 0)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_numeric (v NUMERIC(10, 0) NULL, nn NUMERIC(10, 0) NOT NULL);
-- @queries
-- name: SelectNumeric :many
SELECT v, nn FROM t_numeric;

-- name: InsertNumeric :exec
INSERT INTO t_numeric (v, nn) VALUES (@v, @nn);

-- name: FilterNumeric :many
SELECT v FROM t_numeric WHERE nn = @nn;
