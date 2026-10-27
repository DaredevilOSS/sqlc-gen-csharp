-- @description MERGE as an upsert, with OUTPUT $action
-- @queries
-- name: UpsertAuthor :one
MERGE INTO authors AS target
USING (VALUES (@name, @bio)) AS source (name, bio)
ON target.name = source.name
WHEN MATCHED THEN UPDATE SET bio = source.bio
WHEN NOT MATCHED THEN INSERT (name, bio) VALUES (source.name, source.bio)
OUTPUT $action AS action, INSERTED.id;
