-- @description `MONEY` column: nullable and NOT NULL, read, written and compared to a parameter
-- @schema
CREATE TABLE t_money (v MONEY NULL, nn MONEY NOT NULL);
-- @queries
-- name: SelectMoney :many
SELECT v, nn FROM t_money;

-- name: InsertMoney :exec
INSERT INTO t_money (v, nn) VALUES (@v, @nn);

-- name: FilterMoney :many
SELECT v FROM t_money WHERE nn = @nn;
