-- @description `NCHAR(10)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_nchar (v NCHAR(10) NULL, nn NCHAR(10) NOT NULL);
-- @queries
-- name: SelectNchar :many
SELECT v, nn FROM t_nchar;

-- name: InsertNchar :exec
INSERT INTO t_nchar (v, nn) VALUES (@v, @nn);

-- name: FilterNchar :many
SELECT v FROM t_nchar WHERE nn = @nn;
