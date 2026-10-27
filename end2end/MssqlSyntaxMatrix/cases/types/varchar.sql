-- @description `VARCHAR(50)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_varchar (v VARCHAR(50) NULL, nn VARCHAR(50) NOT NULL);
-- @queries
-- name: SelectVarchar :many
SELECT v, nn FROM t_varchar;

-- name: InsertVarchar :exec
INSERT INTO t_varchar (v, nn) VALUES (@v, @nn);

-- name: FilterVarchar :many
SELECT v FROM t_varchar WHERE nn = @nn;
