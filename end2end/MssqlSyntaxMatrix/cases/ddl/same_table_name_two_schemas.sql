-- @description Two tables with the same name in different schemas
-- @schema
CREATE SCHEMA audit;
GO
CREATE TABLE audit.authors (
    id BIGINT NOT NULL,
    changed_at DATETIME2 NOT NULL
);
-- @queries
-- name: ListAuditAuthors :many
SELECT id, changed_at FROM audit.authors;

-- name: ListAuthors :many
SELECT id, name FROM dbo.authors;
