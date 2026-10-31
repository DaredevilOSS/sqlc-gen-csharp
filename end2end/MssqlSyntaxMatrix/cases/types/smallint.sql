-- @description `SMALLINT` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_smallint (v SMALLINT NULL, nn SMALLINT NOT NULL);
-- @queries
-- name: SelectSmallint :many
SELECT v, nn FROM t_smallint;

-- name: InsertSmallint :exec
INSERT INTO t_smallint (v, nn) VALUES (@v, @nn);

-- name: FilterSmallint :many
SELECT v FROM t_smallint WHERE nn = @nn;
