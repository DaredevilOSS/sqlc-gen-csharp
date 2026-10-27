-- @description `DATE` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_date (v DATE NULL, nn DATE NOT NULL);
-- @queries
-- name: SelectDate :many
SELECT v, nn FROM t_date;

-- name: InsertDate :exec
INSERT INTO t_date (v, nn) VALUES (@v, @nn);

-- name: FilterDate :many
SELECT v FROM t_date WHERE nn = @nn;
