-- @description `NVARCHAR(50)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_nvarchar (v NVARCHAR(50) NULL, nn NVARCHAR(50) NOT NULL);
-- @queries
-- name: SelectNvarchar :many
SELECT v, nn FROM t_nvarchar;

-- name: InsertNvarchar :exec
INSERT INTO t_nvarchar (v, nn) VALUES (@v, @nn);

-- name: FilterNvarchar :many
SELECT v FROM t_nvarchar WHERE nn = @nn;
