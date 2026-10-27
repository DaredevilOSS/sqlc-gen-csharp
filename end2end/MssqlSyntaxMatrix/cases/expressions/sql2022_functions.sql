-- @description GENERATE_SERIES, DATE_BUCKET, TRIM ... FROM and STRING_ESCAPE (SQL Server 2022)
-- @queries
-- name: Series :many
SELECT value FROM GENERATE_SERIES(1, @upto);

-- name: NewFunctions :many
SELECT DATE_BUCKET(WEEK, 1, published_at) AS week_bucket, TRIM(N'x' FROM title) AS trimmed, STRING_ESCAPE(title, 'json') AS escaped FROM books;
