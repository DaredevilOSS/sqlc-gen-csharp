-- @description `DATETIMEOFFSET(7)` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_datetimeoffset (v DATETIMEOFFSET(7) NULL, nn DATETIMEOFFSET(7) NOT NULL);
-- @queries
-- name: SelectDatetimeoffset :many
SELECT v, nn FROM t_datetimeoffset;

-- name: InsertDatetimeoffset :exec
INSERT INTO t_datetimeoffset (v, nn) VALUES (@v, @nn);

-- name: FilterDatetimeoffset :many
SELECT v FROM t_datetimeoffset WHERE nn = @nn;
