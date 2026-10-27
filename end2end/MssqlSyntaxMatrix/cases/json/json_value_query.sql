-- @description JSON_VALUE, JSON_QUERY, ISJSON and JSON_MODIFY on an NVARCHAR(MAX) column
-- @schema
CREATE TABLE settings (id INT IDENTITY PRIMARY KEY, payload NVARCHAR(MAX) NOT NULL CHECK (ISJSON(payload) = 1));
-- @queries
-- name: JsonAccess :many
SELECT id, JSON_VALUE(payload, '$.name') AS name, JSON_QUERY(payload, '$.tags') AS tags, ISJSON(payload) AS is_json,
       JSON_MODIFY(payload, '$.name', @new_name) AS renamed
FROM settings WHERE JSON_VALUE(payload, '$.kind') = @kind;
