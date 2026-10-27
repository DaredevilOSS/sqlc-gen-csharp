-- @description `FLOAT` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_float (v FLOAT NULL, nn FLOAT NOT NULL);
-- @queries
-- name: SelectFloat :many
SELECT v, nn FROM t_float;

-- name: InsertFloat :exec
INSERT INTO t_float (v, nn) VALUES (@v, @nn);

-- name: FilterFloat :many
SELECT v FROM t_float WHERE nn = @nn;
