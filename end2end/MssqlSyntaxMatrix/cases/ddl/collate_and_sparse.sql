-- @description COLLATE and SPARSE column options
-- @schema
CREATE TABLE tags (
    id INT IDENTITY PRIMARY KEY,
    code VARCHAR(20) COLLATE Latin1_General_CS_AS NOT NULL,
    note NVARCHAR(100) SPARSE NULL
);
-- @queries
-- name: ListTags :many
SELECT id, code, note FROM tags;
