-- @description `ROWVERSION` and its legacy synonym `TIMESTAMP`
-- @schema
CREATE TABLE t_rowversion (id INT NOT NULL, rv ROWVERSION);
CREATE TABLE t_timestamp (id INT NOT NULL, ts TIMESTAMP);
-- @queries
-- name: SelectVersions :many
SELECT id, rv FROM t_rowversion;

-- name: SelectTimestamp :many
SELECT id, ts FROM t_timestamp;
