-- @description `TINYINT` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_tinyint (v TINYINT NULL, nn TINYINT NOT NULL);
-- @queries
-- name: SelectTinyint :many
SELECT v, nn FROM t_tinyint;

-- name: InsertTinyint :exec
INSERT INTO t_tinyint (v, nn) VALUES (@v, @nn);

-- name: FilterTinyint :many
SELECT v FROM t_tinyint WHERE nn = @nn;
