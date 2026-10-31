-- @description `SQL_VARIANT` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_sql_variant (v SQL_VARIANT NULL, nn SQL_VARIANT NOT NULL);
-- @queries
-- name: SelectSqlVariant :many
SELECT v, nn FROM t_sql_variant;

-- name: InsertSqlVariant :exec
INSERT INTO t_sql_variant (v, nn) VALUES (@v, @nn);

-- name: FilterSqlVariant :many
SELECT v FROM t_sql_variant WHERE nn = @nn;
