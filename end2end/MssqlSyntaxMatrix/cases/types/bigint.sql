-- @description `BIGINT` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_bigint (v BIGINT NULL, nn BIGINT NOT NULL);
-- @queries
-- name: SelectBigint :many
SELECT v, nn FROM t_bigint;

-- name: InsertBigint :exec
INSERT INTO t_bigint (v, nn) VALUES (@v, @nn);

-- name: FilterBigint :many
SELECT v FROM t_bigint WHERE nn = @nn;
