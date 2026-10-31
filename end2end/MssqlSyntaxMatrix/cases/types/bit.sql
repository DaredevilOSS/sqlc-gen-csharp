-- @description `BIT` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_bit (v BIT NULL, nn BIT NOT NULL);
-- @queries
-- name: SelectBit :many
SELECT v, nn FROM t_bit;

-- name: InsertBit :exec
INSERT INTO t_bit (v, nn) VALUES (@v, @nn);

-- name: FilterBit :many
SELECT v FROM t_bit WHERE nn = @nn;
