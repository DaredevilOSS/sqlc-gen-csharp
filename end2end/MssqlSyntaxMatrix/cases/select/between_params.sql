-- @description BETWEEN with two parameters
-- @queries
-- name: ListBooksInPriceRange :many
SELECT id, price FROM books WHERE price BETWEEN @min_price AND @max_price;
