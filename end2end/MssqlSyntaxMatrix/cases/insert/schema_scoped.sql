-- @description INSERT into a table in a user-defined schema
-- @schema
CREATE SCHEMA logging;
GO
CREATE TABLE logging.entries (id INT IDENTITY PRIMARY KEY, message NVARCHAR(200) NOT NULL);
-- @queries
-- name: CreateLog :exec
INSERT INTO logging.entries (message) VALUES (@message);
