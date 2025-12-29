CREATE TABLE IF NOT EXISTS customers (
    customer_id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    email TEXT UNIQUE NOT NULL,
    phone TEXT NOT NULL,
    address TEXT,
    registered_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS products (
    product_id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    category TEXT NOT NULL,
    unit_price REAL NOT NULL,
    stock_quantity INTEGER NOT NULL DEFAULT 0,
    description TEXT,
    added_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS orders (
    order_id        INTEGER        PRIMARY KEY AUTOINCREMENT,
    customer_id     INTEGER        NOT NULL,
    ordered_at      TEXT           NOT NULL DEFAULT (datetime('now')),
    order_state     TEXT           NOT NULL CHECK (order_state IN ('Pending', 'Delivered', 'Cancelled')),
    total_amount    REAL           NOT NULL,
    CONSTRAINT      fk_customer    FOREIGN KEY (customer_id) REFERENCES customers(customer_id)
);

CREATE TABLE IF NOT EXISTS order_items (
    order_item_id   INTEGER        PRIMARY KEY AUTOINCREMENT,
    order_id        INTEGER        NOT NULL,
    product_id      INTEGER        NOT NULL,
    quantity        INTEGER        NOT NULL CHECK (quantity > 0),
    unit_price      REAL           NOT NULL,
    CONSTRAINT      fk_order       FOREIGN KEY (order_id)   REFERENCES orders(order_id),
    CONSTRAINT      fk_product     FOREIGN KEY (product_id) REFERENCES products(product_id)
);

CREATE INDEX idx_customer_id_orders ON orders (customer_id); -- lookup of orders by customer_id
CREATE INDEX idx_order_id_order_items ON order_items (order_id); -- lookup of order_items by order_id