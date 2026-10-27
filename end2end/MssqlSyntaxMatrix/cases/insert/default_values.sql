-- @description INSERT ... DEFAULT VALUES and explicit DEFAULT
-- @schema
CREATE TABLE events (id INT IDENTITY PRIMARY KEY, created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME());
-- @queries
-- name: CreateEmptyEvent :exec
INSERT INTO events DEFAULT VALUES;

-- name: CreateEventWithDefault :exec
INSERT INTO events (created_at) VALUES (DEFAULT);
