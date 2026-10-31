-- @description UPDATE with SET parameters and WHERE
-- @queries
-- name: UpdateBio :execrows
UPDATE authors SET bio = @bio, birth_date = @birth_date WHERE id = @id;
