-- @description `SYSNAME` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_sysname (v SYSNAME NULL, nn SYSNAME NOT NULL);
-- @queries
-- name: SelectSysname :many
SELECT v, nn FROM t_sysname;

-- name: InsertSysname :exec
INSERT INTO t_sysname (v, nn) VALUES (@v, @nn);

-- name: FilterSysname :many
SELECT v FROM t_sysname WHERE nn = @nn;
