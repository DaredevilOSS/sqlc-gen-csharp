-- @description `DATETIME2(7)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_datetime2 (v DATETIME2(7) NULL, nn DATETIME2(7) NOT NULL);
-- @queries
-- name: SelectDatetime2 :many
SELECT v, nn FROM t_datetime2;

-- name: InsertDatetime2 :exec
INSERT INTO t_datetime2 (v, nn) VALUES (@v, @nn);

-- name: FilterDatetime2 :many
SELECT v FROM t_datetime2 WHERE nn = @nn;
