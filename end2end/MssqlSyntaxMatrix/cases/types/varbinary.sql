-- @description `VARBINARY(50)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_varbinary (v VARBINARY(50) NULL, nn VARBINARY(50) NOT NULL);
-- @queries
-- name: SelectVarbinary :many
SELECT v, nn FROM t_varbinary;

-- name: InsertVarbinary :exec
INSERT INTO t_varbinary (v, nn) VALUES (@v, @nn);

-- name: FilterVarbinary :many
SELECT v FROM t_varbinary WHERE nn = @nn;
