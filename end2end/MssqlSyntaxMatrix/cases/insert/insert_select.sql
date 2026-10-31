-- @description INSERT ... SELECT with a parameter
-- @queries
-- name: CopyBooksToAuthor :execrows
INSERT INTO books (author_id, title, price) SELECT @target_author_id, title, price FROM books WHERE author_id = @source_author_id;
