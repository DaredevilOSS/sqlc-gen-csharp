CREATE SCHEMA sales;

CREATE TABLE IF NOT EXISTS sales.customers (
    customer_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    phone VARCHAR(50) NOT NULL,
    address VARCHAR(255),
    registered_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS sales.products (
    product_id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    category VARCHAR(100) NOT NULL,
    unit_price DECIMAL(10, 2) NOT NULL,
    stock_quantity INT NOT NULL DEFAULT 0,
    description TEXT,
    added_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS sales.orders (
    order_id        UUID           PRIMARY KEY DEFAULT uuid_generate_v4(),
    customer_id     INT            NOT NULL,
    ordered_at      TIMESTAMP      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    order_state     VARCHAR(10)    NOT NULL CHECK (order_state IN ('Pending', 'Delivered', 'Cancelled')),
    total_amount    DECIMAL(10,2)  NOT NULL,
    CONSTRAINT      fk_customer    FOREIGN KEY (customer_id) REFERENCES sales.customers(customer_id)
);

CREATE TABLE IF NOT EXISTS sales.order_items (
    order_item_id   UUID           PRIMARY KEY DEFAULT uuid_generate_v4(),
    order_id        UUID           NOT NULL,
    product_id      INT            NOT NULL,
    quantity        INT            NOT NULL CHECK (quantity > 0),
    unit_price      DECIMAL(10,2)  NOT NULL,
    CONSTRAINT      fk_order       FOREIGN KEY (order_id)   REFERENCES sales.orders(order_id),
    CONSTRAINT      fk_product     FOREIGN KEY (product_id) REFERENCES sales.products(product_id)
);

