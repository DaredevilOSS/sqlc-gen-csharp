-- @description System-versioned temporal table and FOR SYSTEM_TIME AS OF
-- @schema
CREATE TABLE prices (
    id INT NOT NULL PRIMARY KEY CLUSTERED,
    amount DECIMAL(10, 2) NOT NULL,
    valid_from DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    valid_to DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME (valid_from, valid_to)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.prices_history));
-- @queries
-- name: ListPricesAsOf :many
SELECT id, amount FROM prices FOR SYSTEM_TIME AS OF @as_of;
