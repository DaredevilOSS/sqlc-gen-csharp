-- @description MERGE with WHEN NOT MATCHED BY SOURCE THEN DELETE
-- @schema
CREATE TABLE tags (name NVARCHAR(50) NOT NULL PRIMARY KEY);
-- @queries
-- name: SyncTags :execrows
MERGE INTO tags AS t
USING (SELECT value AS name FROM STRING_SPLIT(@names, ',')) AS s ON t.name = s.name
WHEN NOT MATCHED BY TARGET THEN INSERT (name) VALUES (s.name)
WHEN NOT MATCHED BY SOURCE THEN DELETE;
