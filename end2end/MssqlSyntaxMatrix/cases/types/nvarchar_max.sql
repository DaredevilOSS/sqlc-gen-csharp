-- @description `NVARCHAR(MAX)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_nvarchar_max (v NVARCHAR(MAX) NULL, nn NVARCHAR(MAX) NOT NULL);
-- @queries
-- name: SelectNvarcharMax :many
SELECT v, nn FROM t_nvarchar_max;

-- name: InsertNvarcharMax :exec
INSERT INTO t_nvarchar_max (v, nn) VALUES (@v, @nn);

-- name: FilterNvarcharMax :many
SELECT v FROM t_nvarchar_max WHERE nn = @nn;
