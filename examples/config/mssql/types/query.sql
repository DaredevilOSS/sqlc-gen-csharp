/* Numeric types */

-- name: InsertMssqlNumericTypes :exec
INSERT INTO mssql_numeric_types
(
    c_bit,
    c_tinyint,
    c_smallint,
    c_int,
    c_bigint,
    c_real,
    c_float,
    c_decimal,
    c_numeric,
    c_money,
    c_smallmoney
)
VALUES (@c_bit, @c_tinyint, @c_smallint, @c_int, @c_bigint, @c_real, @c_float, @c_decimal, @c_numeric, @c_money, @c_smallmoney);

-- name: InsertMssqlNumericTypesBatch :copyfrom
INSERT INTO mssql_numeric_types
(
    c_bit,
    c_tinyint,
    c_smallint,
    c_int,
    c_bigint,
    c_real,
    c_float,
    c_decimal,
    c_numeric,
    c_money,
    c_smallmoney
)
VALUES (@c_bit, @c_tinyint, @c_smallint, @c_int, @c_bigint, @c_real, @c_float, @c_decimal, @c_numeric, @c_money, @c_smallmoney);

-- name: GetMssqlNumericTypes :one
SELECT TOP 1 * FROM mssql_numeric_types;

-- name: GetMssqlNumericTypesCnt :one
SELECT TOP 1
    COUNT(*) AS cnt,
    c_bit,
    c_tinyint,
    c_smallint,
    c_int,
    c_bigint,
    c_real,
    c_float,
    c_decimal,
    c_numeric,
    c_money,
    c_smallmoney
FROM mssql_numeric_types
GROUP BY
    c_bit,
    c_tinyint,
    c_smallint,
    c_int,
    c_bigint,
    c_real,
    c_float,
    c_decimal,
    c_numeric,
    c_money,
    c_smallmoney;

-- name: TruncateMssqlNumericTypes :exec
TRUNCATE TABLE mssql_numeric_types;

/* String types */

-- name: InsertMssqlStringTypes :exec
INSERT INTO mssql_string_types
(
    c_char,
    c_nchar,
    c_varchar,
    c_nvarchar,
    c_varchar_max,
    c_nvarchar_max
)
VALUES (@c_char, @c_nchar, @c_varchar, @c_nvarchar, @c_varchar_max, @c_nvarchar_max);

-- name: InsertMssqlStringTypesBatch :copyfrom
INSERT INTO mssql_string_types
(
    c_char,
    c_nchar,
    c_varchar,
    c_nvarchar,
    c_varchar_max,
    c_nvarchar_max
)
VALUES (@c_char, @c_nchar, @c_varchar, @c_nvarchar, @c_varchar_max, @c_nvarchar_max);

-- name: GetMssqlStringTypes :one
SELECT TOP 1 * FROM mssql_string_types;

-- name: GetMssqlStringTypesCnt :one
SELECT TOP 1
    COUNT(*) AS cnt,
    c_char,
    c_nchar,
    c_varchar,
    c_nvarchar,
    c_varchar_max,
    c_nvarchar_max
FROM mssql_string_types
GROUP BY
    c_char,
    c_nchar,
    c_varchar,
    c_nvarchar,
    c_varchar_max,
    c_nvarchar_max;

-- name: TruncateMssqlStringTypes :exec
TRUNCATE TABLE mssql_string_types;

/* Datetime types */

-- name: InsertMssqlDatetimeTypes :exec
INSERT INTO mssql_datetime_types
(
    c_date,
    c_datetime,
    c_datetime2,
    c_smalldatetime,
    c_datetimeoffset,
    c_time
)
VALUES (@c_date, @c_datetime, @c_datetime2, @c_smalldatetime, @c_datetimeoffset, @c_time);

-- name: InsertMssqlDatetimeTypesBatch :copyfrom
INSERT INTO mssql_datetime_types
(
    c_date,
    c_datetime,
    c_datetime2,
    c_smalldatetime,
    c_datetimeoffset,
    c_time
)
VALUES (@c_date, @c_datetime, @c_datetime2, @c_smalldatetime, @c_datetimeoffset, @c_time);

-- name: GetMssqlDatetimeTypes :one
SELECT TOP 1 * FROM mssql_datetime_types;

-- name: GetMssqlDatetimeTypesCnt :one
SELECT TOP 1
    COUNT(*) AS cnt,
    c_date,
    c_datetime,
    c_datetime2,
    c_smalldatetime,
    c_datetimeoffset,
    c_time
FROM mssql_datetime_types
GROUP BY
    c_date,
    c_datetime,
    c_datetime2,
    c_smalldatetime,
    c_datetimeoffset,
    c_time;

-- name: TruncateMssqlDatetimeTypes :exec
TRUNCATE TABLE mssql_datetime_types;

/* Binary types */

-- name: InsertMssqlBinaryTypes :exec
INSERT INTO mssql_binary_types
(
    c_binary,
    c_varbinary,
    c_varbinary_max,
    c_uniqueidentifier
)
VALUES (@c_binary, @c_varbinary, @c_varbinary_max, @c_uniqueidentifier);

-- name: InsertMssqlBinaryTypesBatch :copyfrom
INSERT INTO mssql_binary_types
(
    c_binary,
    c_varbinary,
    c_varbinary_max,
    c_uniqueidentifier
)
VALUES (@c_binary, @c_varbinary, @c_varbinary_max, @c_uniqueidentifier);

-- name: GetMssqlBinaryTypes :one
SELECT TOP 1 * FROM mssql_binary_types;

-- name: GetMssqlBinaryTypesCnt :one
SELECT TOP 1
    COUNT(*) AS cnt,
    c_binary,
    c_varbinary,
    c_varbinary_max,
    c_uniqueidentifier
FROM mssql_binary_types
GROUP BY
    c_binary,
    c_varbinary,
    c_varbinary_max,
    c_uniqueidentifier;

-- name: TruncateMssqlBinaryTypes :exec
TRUNCATE TABLE mssql_binary_types;

/* Functions */

-- name: GetMssqlFunctions :one
SELECT
    MAX(c_int) AS max_int,
    MAX(c_varchar) AS max_varchar,
    MAX(c_datetime2) AS max_datetime2
FROM mssql_numeric_types
CROSS JOIN mssql_string_types
CROSS JOIN mssql_datetime_types;
