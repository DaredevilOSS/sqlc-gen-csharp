-- @description GETDATE, SYSDATETIME, SYSDATETIMEOFFSET, DATEADD, DATEDIFF, DATEDIFF_BIG, DATEPART, DATENAME, EOMONTH, DATEFROMPARTS, DATETRUNC
-- @queries
-- name: DateFunctions :many
SELECT GETDATE() AS now_local, GETUTCDATE() AS now_utc, SYSDATETIME() AS precise, SYSDATETIMEOFFSET() AS with_offset,
       DATEADD(DAY, @days, published_at) AS shifted, DATEDIFF(DAY, published_at, @until) AS days_between,
       DATEDIFF_BIG(SECOND, published_at, SYSUTCDATETIME()) AS seconds_since, DATEPART(YEAR, published_at) AS year_part,
       DATENAME(MONTH, published_at) AS month_name, YEAR(published_at) AS y, EOMONTH(published_at) AS month_end,
       DATEFROMPARTS(2024, 1, 31) AS fixed_date, DATETRUNC(MONTH, published_at) AS month_start
FROM books;
