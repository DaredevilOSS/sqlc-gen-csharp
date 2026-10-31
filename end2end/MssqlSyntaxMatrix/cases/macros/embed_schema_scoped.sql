-- @description sqlc.embed of a table in a user-defined schema
-- @schema
CREATE SCHEMA sales;
GO
CREATE TABLE sales.orders (id INT IDENTITY PRIMARY KEY, author_id BIGINT NOT NULL, total MONEY NOT NULL);
-- @queries
-- name: OrdersWithAuthor :many
SELECT sqlc.embed(orders), sqlc.embed(authors) FROM sales.orders JOIN authors ON authors.id = orders.author_id;
