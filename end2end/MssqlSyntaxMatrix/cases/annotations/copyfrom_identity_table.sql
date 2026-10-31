-- @description `:copyfrom` into a table with an IDENTITY column
-- @queries
-- name: CopyBooks :copyfrom
INSERT INTO books (author_id, title, price, published_at, is_available) VALUES (@author_id, @title, @price, @published_at, @is_available);
