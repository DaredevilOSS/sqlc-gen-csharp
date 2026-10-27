-- @description Named DEFAULT constraints with GETDATE() and NEWID()
-- @schema
CREATE TABLE events (
    id INT IDENTITY CONSTRAINT pk_events PRIMARY KEY,
    created_at DATETIME2 NOT NULL CONSTRAINT df_events_created_at DEFAULT (SYSUTCDATETIME()),
    correlation_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT df_events_correlation DEFAULT (NEWID())
);
-- @queries
-- name: ListEvents :many
SELECT id, created_at, correlation_id FROM events;
