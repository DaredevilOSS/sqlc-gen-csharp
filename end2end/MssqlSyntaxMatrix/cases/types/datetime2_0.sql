-- @description `DATETIME2(0)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_datetime2_0 (v DATETIME2(0) NULL, nn DATETIME2(0) NOT NULL);
-- @queries
-- name: SelectDatetime20 :many
SELECT v, nn FROM t_datetime2_0;

-- name: InsertDatetime20 :exec
INSERT INTO t_datetime2_0 (v, nn) VALUES (@v, @nn);

-- name: FilterDatetime20 :many
SELECT v FROM t_datetime2_0 WHERE nn = @nn;
