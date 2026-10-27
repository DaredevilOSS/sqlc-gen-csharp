-- @description The type expression keeps lengths, MAX, precision and scale
-- @schema
CREATE TABLE t_precise (d DECIMAL(38, 10) NOT NULL, n NVARCHAR(4000) NOT NULL, t TIME(0) NOT NULL, dt DATETIME2(3) NOT NULL);
-- @queries
-- name: SelectPrecise :many
SELECT d, n, t, dt FROM t_precise WHERE d = @d AND n = @n;
