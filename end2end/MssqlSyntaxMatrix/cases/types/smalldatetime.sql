-- @description `SMALLDATETIME` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_smalldatetime (v SMALLDATETIME NULL, nn SMALLDATETIME NOT NULL);
-- @queries
-- name: SelectSmalldatetime :many
SELECT v, nn FROM t_smalldatetime;

-- name: InsertSmalldatetime :exec
INSERT INTO t_smalldatetime (v, nn) VALUES (@v, @nn);

-- name: FilterSmalldatetime :many
SELECT v FROM t_smalldatetime WHERE nn = @nn;
