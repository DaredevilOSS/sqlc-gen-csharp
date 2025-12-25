-- name: GetCustomerOrders :many
SELECT 
    o.order_id, ordered_at, order_state, total_amount, order_item_id, i.quantity, i.unit_price, 
    p.product_id, p.name as product_name, p.category as product_category
FROM orders o
JOIN order_items i
USING (order_id)
JOIN products p
USING (product_id)
WHERE o.customer_id = sqlc.arg('customer_id')
ORDER BY o.ordered_at DESC
LIMIT sqlc.arg('limit') OFFSET sqlc.arg('offset');

-- name: AddCustomers :copyfrom
INSERT INTO customers (name, email, phone, address, registered_at) VALUES (?, ?, ?, ?, ?);

-- name: AddProducts :copyfrom
INSERT INTO products (name, category, unit_price, stock_quantity, description) VALUES (?, ?, ?, ?, ?);

-- name: AddOrders :copyfrom
INSERT INTO orders (customer_id, order_state, total_amount) VALUES (?, ?, ?);

-- name: AddOrderItems :copyfrom
INSERT INTO order_items (order_id, product_id, quantity, unit_price) VALUES (?, ?, ?, ?);

-- name: GetCustomerIds :many
SELECT customer_id FROM customers ORDER BY customer_id LIMIT ?;

-- name: GetProductIds :many
SELECT product_id FROM products ORDER BY product_id LIMIT ?;

-- name: GetOrderIds :many
SELECT order_id FROM orders ORDER BY order_id LIMIT ?;

-- name: GetOrderItemsCount :one
SELECT COUNT(*) AS cnt FROM order_items;

-- name: GetOrderAmounts :many
SELECT order_id, total_amount FROM orders WHERE order_id IN (/*SLICE:order_ids*/?);

-- name: GetProductPrices :many
SELECT product_id, unit_price FROM products WHERE product_id IN (/*SLICE:product_ids*/?);

