-- @description `SMALLMONEY` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_smallmoney (v SMALLMONEY NULL, nn SMALLMONEY NOT NULL);
-- @queries
-- name: SelectSmallmoney :many
SELECT v, nn FROM t_smallmoney;

-- name: InsertSmallmoney :exec
INSERT INTO t_smallmoney (v, nn) VALUES (@v, @nn);

-- name: FilterSmallmoney :many
SELECT v FROM t_smallmoney WHERE nn = @nn;
