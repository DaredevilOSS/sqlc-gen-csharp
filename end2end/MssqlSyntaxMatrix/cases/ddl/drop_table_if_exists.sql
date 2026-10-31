-- @description DROP TABLE IF EXISTS followed by a re-create with a different shape
-- @schema
CREATE TABLE scratch (id INT NOT NULL);
DROP TABLE IF EXISTS scratch;
CREATE TABLE scratch (id BIGINT NOT NULL, label NVARCHAR(10) NOT NULL);
-- @queries
-- name: ListScratch :many
SELECT id, label FROM scratch;
