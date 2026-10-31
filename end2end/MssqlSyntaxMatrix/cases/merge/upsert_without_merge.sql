-- @description The UPDATE-then-INSERT upsert pattern with @@ROWCOUNT
-- @queries
-- name: UpsertBio :exec
UPDATE authors SET bio = @bio WHERE name = @name;
IF @@ROWCOUNT = 0
    INSERT INTO authors (name, bio) VALUES (@name, @bio);
