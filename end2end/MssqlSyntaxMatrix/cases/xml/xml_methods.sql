-- @description XML column with the value(), query(), exist() and nodes() methods
-- @schema
CREATE TABLE orders_xml (id INT IDENTITY PRIMARY KEY, doc XML NOT NULL);
-- @queries
-- name: XmlAccess :many
SELECT id, doc.value('(/order/@id)[1]', 'INT') AS order_id, doc.query('/order/item') AS items
FROM orders_xml WHERE doc.exist('/order[@status=sql:variable("@status")]') = 1;

-- name: XmlNodes :many
SELECT o.id, i.n.value('@sku', 'NVARCHAR(20)') AS sku FROM orders_xml o CROSS APPLY o.doc.nodes('/order/item') AS i(n);
