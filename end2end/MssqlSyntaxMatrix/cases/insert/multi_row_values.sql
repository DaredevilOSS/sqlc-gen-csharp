-- @description INSERT with several VALUES rows
-- @queries
-- name: CreateTwoAuthors :exec
INSERT INTO authors (name, bio) VALUES (@first_name, @first_bio), (@second_name, @second_bio);
