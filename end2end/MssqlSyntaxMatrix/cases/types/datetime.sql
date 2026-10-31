-- @description `DATETIME` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_datetime (v DATETIME NULL, nn DATETIME NOT NULL);
-- @queries
-- name: SelectDatetime :many
SELECT v, nn FROM t_datetime;

-- name: InsertDatetime :exec
INSERT INTO t_datetime (v, nn) VALUES (@v, @nn);

-- name: FilterDatetime :many
SELECT v FROM t_datetime WHERE nn = @nn;
