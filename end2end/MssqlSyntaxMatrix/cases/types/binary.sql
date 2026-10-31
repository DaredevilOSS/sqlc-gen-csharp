-- @description `BINARY(16)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_binary (v BINARY(16) NULL, nn BINARY(16) NOT NULL);
-- @queries
-- name: SelectBinary :many
SELECT v, nn FROM t_binary;

-- name: InsertBinary :exec
INSERT INTO t_binary (v, nn) VALUES (@v, @nn);

-- name: FilterBinary :many
SELECT v FROM t_binary WHERE nn = @nn;
