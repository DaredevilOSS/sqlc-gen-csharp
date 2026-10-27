-- @description `VARBINARY(MAX)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_varbinary_max (v VARBINARY(MAX) NULL, nn VARBINARY(MAX) NOT NULL);
-- @queries
-- name: SelectVarbinaryMax :many
SELECT v, nn FROM t_varbinary_max;

-- name: InsertVarbinaryMax :exec
INSERT INTO t_varbinary_max (v, nn) VALUES (@v, @nn);

-- name: FilterVarbinaryMax :many
SELECT v FROM t_varbinary_max WHERE nn = @nn;
