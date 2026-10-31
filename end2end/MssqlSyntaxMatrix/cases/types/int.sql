-- @description `INT` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_int (v INT NULL, nn INT NOT NULL);
-- @queries
-- name: SelectInt :many
SELECT v, nn FROM t_int;

-- name: InsertInt :exec
INSERT INTO t_int (v, nn) VALUES (@v, @nn);

-- name: FilterInt :many
SELECT v FROM t_int WHERE nn = @nn;
