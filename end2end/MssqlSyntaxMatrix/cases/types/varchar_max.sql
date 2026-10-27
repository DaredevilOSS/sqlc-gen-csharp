-- @description `VARCHAR(MAX)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_varchar_max (v VARCHAR(MAX) NULL, nn VARCHAR(MAX) NOT NULL);
-- @queries
-- name: SelectVarcharMax :many
SELECT v, nn FROM t_varchar_max;

-- name: InsertVarcharMax :exec
INSERT INTO t_varchar_max (v, nn) VALUES (@v, @nn);

-- name: FilterVarcharMax :many
SELECT v FROM t_varchar_max WHERE nn = @nn;
