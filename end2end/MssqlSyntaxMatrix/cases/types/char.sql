-- @description `CHAR(10)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_char (v CHAR(10) NULL, nn CHAR(10) NOT NULL);
-- @queries
-- name: SelectChar :many
SELECT v, nn FROM t_char;

-- name: InsertChar :exec
INSERT INTO t_char (v, nn) VALUES (@v, @nn);

-- name: FilterChar :many
SELECT v FROM t_char WHERE nn = @nn;
