-- name: GetCustomerOrders :many
SELECT 
    o.order_id, ordered_at, order_state, total_amount, order_item_id, i.quantity, i.unit_price, 
    p.product_id, p.name as product_name, p.category as product_category
FROM sales.orders o
JOIN sales.order_items i
USING (order_id)
JOIN sales.products p
USING (product_id)
WHERE o.customer_id = ?
ORDER BY o.ordered_at DESC
LIMIT ? OFFSET ?;

-- name: AddCustomers :copyfrom
INSERT INTO sales.customers (name, email, phone, address, registered_at) VALUES (?, ?, ?, ?, ?);

-- name: AddProducts :copyfrom
INSERT INTO sales.products (name, category, unit_price, stock_quantity, description) VALUES (?, ?, ?, ?, ?);

-- name: AddOrders :copyfrom
INSERT INTO sales.orders (customer_id, order_state, total_amount) VALUES (?, ?, ?);

-- name: AddOrderItems :copyfrom
INSERT INTO sales.order_items (order_id, product_id, quantity, unit_price) VALUES (?, ?, ?, ?);

-- name: GetCustomerIds :many
SELECT customer_id FROM sales.customers ORDER BY customer_id LIMIT ?;

-- name: GetProductIds :many
SELECT product_id FROM sales.products ORDER BY product_id LIMIT ?;

-- name: GetOrderIds :many
SELECT order_id FROM sales.orders ORDER BY order_id LIMIT ?;

-- name: GetOrderItemsCount :one
SELECT COUNT(*) AS cnt FROM sales.order_items;

-- name: GetOrderAmounts :many
SELECT order_id, total_amount FROM sales.orders WHERE order_id IN (/*SLICE:order_ids*/?);

-- name: GetProductPrices :many
SELECT product_id, unit_price FROM sales.products WHERE product_id IN (/*SLICE:product_ids*/?);
