-- @description CONCAT, CONCAT_WS, FORMAT, QUOTENAME, TRANSLATE
-- @queries
-- name: Formatting :many
SELECT CONCAT(name, N' (', id, N')') AS label, CONCAT_WS(N', ', name, bio) AS joined,
       FORMAT(birth_date, 'yyyy-MM-dd') AS formatted, QUOTENAME(name) AS quoted, TRANSLATE(name, N'abc', N'xyz') AS translated
FROM authors;
