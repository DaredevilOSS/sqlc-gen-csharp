-- @description ROWVERSION column used for optimistic concurrency
-- @schema
CREATE TABLE documents (
    id INT IDENTITY PRIMARY KEY,
    body NVARCHAR(MAX) NOT NULL,
    version ROWVERSION NOT NULL
);
-- @queries
-- name: GetDocument :one
SELECT id, body, version FROM documents WHERE id = @id;

-- name: UpdateDocument :execrows
UPDATE documents SET body = @body WHERE id = @id AND version = @version;
