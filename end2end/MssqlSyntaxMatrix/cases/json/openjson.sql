-- @description OPENJSON with an explicit WITH schema, fed by a parameter
-- @queries
-- name: ParseItems :many
SELECT item_id, quantity FROM OPENJSON(@items) WITH (item_id INT '$.id', quantity INT '$.qty');

-- name: AuthorsByJsonIds :many
SELECT id, name FROM authors WHERE id IN (SELECT CAST(value AS BIGINT) FROM OPENJSON(@ids));
