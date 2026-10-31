-- @description `UNIQUEIDENTIFIER` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_uniqueidentifier (v UNIQUEIDENTIFIER NULL, nn UNIQUEIDENTIFIER NOT NULL);
-- @queries
-- name: SelectUniqueidentifier :many
SELECT v, nn FROM t_uniqueidentifier;

-- name: InsertUniqueidentifier :exec
INSERT INTO t_uniqueidentifier (v, nn) VALUES (@v, @nn);

-- name: FilterUniqueidentifier :many
SELECT v FROM t_uniqueidentifier WHERE nn = @nn;
