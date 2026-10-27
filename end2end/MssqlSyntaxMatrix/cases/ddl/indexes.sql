-- @description CREATE INDEX statements (filtered, INCLUDE, unique nonclustered) are accepted and ignored
-- @schema
CREATE NONCLUSTERED INDEX ix_books_title ON books (title) INCLUDE (price);
CREATE UNIQUE NONCLUSTERED INDEX ux_books_author_title ON books (author_id, title);
CREATE INDEX ix_books_available ON books (published_at) WHERE is_available = 1;
-- @queries
-- name: ListAvailableBooks :many
SELECT id, title FROM books WHERE is_available = 1;
