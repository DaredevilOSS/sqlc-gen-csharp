-- @description CAST, CONVERT (with a style), TRY_CAST, TRY_CONVERT, PARSE and TRY_PARSE
-- @queries
-- name: Conversions :many
SELECT CAST(id AS NVARCHAR(20)) AS id_text,
       CONVERT(VARCHAR(10), published_at, 23) AS iso_date,
       TRY_CAST(title AS INT) AS title_number,
       TRY_CONVERT(DECIMAL(10, 2), @amount) AS amount,
       PARSE(N'2024-01-02' AS DATE USING 'en-US') AS parsed,
       TRY_PARSE(@text AS DATETIME2) AS maybe_date
FROM books;
