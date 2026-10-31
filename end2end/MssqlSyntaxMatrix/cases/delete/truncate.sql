-- @description TRUNCATE TABLE
-- @schema
CREATE TABLE books_archive (id INT NOT NULL);
-- @queries
-- name: TruncateBooksArchive :exec
TRUNCATE TABLE books_archive;
