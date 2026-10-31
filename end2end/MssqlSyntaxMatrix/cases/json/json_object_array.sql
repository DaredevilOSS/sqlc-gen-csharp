-- @description JSON_OBJECT and JSON_ARRAY constructors (SQL Server 2022)
-- @queries
-- name: AuthorJson :many
SELECT JSON_OBJECT('id': id, 'name': name) AS obj, JSON_ARRAY(id, name) AS arr FROM authors;
