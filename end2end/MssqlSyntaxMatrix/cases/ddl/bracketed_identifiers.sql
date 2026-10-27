-- @description Bracket-quoted table and column names with spaces and reserved words
-- @schema
CREATE TABLE [order details] (
    [order id] INT NOT NULL,
    [select] NVARCHAR(10) NULL,
    [Unit Price] MONEY NOT NULL
);
-- @queries
-- name: ListOrderDetails :many
SELECT [order id], [select], [Unit Price] FROM [order details];
