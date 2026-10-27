-- @description `:execlastid` via OUTPUT INSERTED.id on an IDENTITY column
-- @queries
-- name: CreateBookReturnId :execlastid
INSERT INTO books (author_id, title) OUTPUT INSERTED.id VALUES (@author_id, @title);
