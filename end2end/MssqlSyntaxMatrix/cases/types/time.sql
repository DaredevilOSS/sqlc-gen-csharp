-- @description `TIME(3)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_time (v TIME(3) NULL, nn TIME(3) NOT NULL);
-- @queries
-- name: SelectTime :many
SELECT v, nn FROM t_time;

-- name: InsertTime :exec
INSERT INTO t_time (v, nn) VALUES (@v, @nn);

-- name: FilterTime :many
SELECT v FROM t_time WHERE nn = @nn;
