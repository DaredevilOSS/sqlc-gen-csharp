-- @description CHECK and UNIQUE constraints, column and table level
-- @schema
CREATE TABLE products (
    id INT IDENTITY PRIMARY KEY,
    sku VARCHAR(20) NOT NULL UNIQUE,
    price DECIMAL(10, 2) NOT NULL CHECK (price >= 0),
    discount DECIMAL(10, 2) NULL,
    CONSTRAINT ck_products_discount CHECK (discount IS NULL OR discount <= price),
    CONSTRAINT uq_products_sku_price UNIQUE (sku, price)
);
-- @queries
-- name: ListProducts :many
SELECT id, sku, price, discount FROM products;
