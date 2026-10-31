-- @description IDENTITY(seed, increment) column on a non-default seed
-- @schema
CREATE TABLE invoices (
    id INT IDENTITY(1000, 5) NOT NULL PRIMARY KEY,
    number NVARCHAR(20) NOT NULL
);
-- @queries
-- name: ListInvoices :many
SELECT id, number FROM invoices;
