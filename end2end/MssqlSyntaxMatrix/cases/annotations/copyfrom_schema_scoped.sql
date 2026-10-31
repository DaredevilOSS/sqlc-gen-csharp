-- @description `:copyfrom` into a table in a user-defined schema
-- @schema
CREATE SCHEMA logging;
GO
CREATE TABLE logging.entries (id INT IDENTITY PRIMARY KEY, message NVARCHAR(200) NOT NULL, level TINYINT NOT NULL);
-- @queries
-- name: CopyEntries :copyfrom
INSERT INTO logging.entries (message, level) VALUES (@message, @level);
