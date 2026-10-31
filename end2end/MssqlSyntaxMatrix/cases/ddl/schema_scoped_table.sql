-- @description Table in a user-defined schema (CREATE SCHEMA in its own batch)
-- @schema
CREATE SCHEMA sales;
GO
CREATE TABLE sales.orders (
    id INT IDENTITY PRIMARY KEY,
    author_id BIGINT NOT NULL,
    total MONEY NOT NULL
);
-- @queries
-- name: ListOrders :many
SELECT id, author_id, total FROM sales.orders;

-- name: ListOrdersWithAuthor :many
SELECT o.id, a.name FROM sales.orders o JOIN dbo.authors a ON a.id = o.author_id;
