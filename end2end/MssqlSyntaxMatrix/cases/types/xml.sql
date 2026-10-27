-- @description `XML` column: nullable and NOT NULL, read, written
-- @schema
CREATE TABLE t_xml (v XML NULL, nn XML NOT NULL);
-- @queries
-- name: SelectXml :many
SELECT v, nn FROM t_xml;

-- name: InsertXml :exec
INSERT INTO t_xml (v, nn) VALUES (@v, @nn);
