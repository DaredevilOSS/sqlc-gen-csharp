-- @description `DECIMAL(18, 4)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_decimal (v DECIMAL(18, 4) NULL, nn DECIMAL(18, 4) NOT NULL);
-- @queries
-- name: SelectDecimal :many
SELECT v, nn FROM t_decimal;

-- name: InsertDecimal :exec
INSERT INTO t_decimal (v, nn) VALUES (@v, @nn);

-- name: FilterDecimal :many
SELECT v FROM t_decimal WHERE nn = @nn;
