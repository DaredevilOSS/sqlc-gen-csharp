/* Numeric types */

-- name: InsertMysqlNumericTypes :exec
INSERT INTO mysql_numeric_types 
(
    c_bool,
    c_boolean,
    c_tinyint,
    c_smallint,
    c_mediumint,
    c_int,
    c_integer,
    c_bigint, 
    c_decimal, 
    c_dec, 
    c_numeric, 
    c_fixed, 
    c_float, 
    c_double, 
    c_double_precision
) 
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: InsertMysqlNumericTypesBatch :copyfrom
INSERT INTO mysql_numeric_types 
(
    c_bool,
    c_boolean,
    c_tinyint,
    c_smallint,
    c_mediumint,
    c_int,
    c_integer,
    c_bigint, 
    c_float, 
    c_numeric, 
    c_decimal, 
    c_dec, 
    c_fixed, 
    c_double, 
    c_double_precision
) 
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: GetMysqlNumericTypes :one
SELECT * FROM mysql_numeric_types LIMIT 1;

-- name: GetMysqlNumericTypesCnt :one
SELECT
    COUNT(*) AS cnt,
    c_bool,
    c_boolean,
    c_tinyint,
    c_smallint,
    c_mediumint,
    c_int,
    c_integer,
    c_bigint,
    c_float,
    c_numeric,
    c_decimal,
    c_dec,
    c_fixed,
    c_double,
    c_double_precision
FROM mysql_numeric_types
GROUP BY
    c_bool,
    c_boolean,
    c_tinyint,
    c_smallint,
    c_mediumint,
    c_int,
    c_integer,
    c_bigint,
    c_float,
    c_numeric,
    c_decimal,
    c_dec,
    c_fixed,
    c_double,
    c_double_precision
LIMIT 1;

-- name: TruncateMysqlNumericTypes :exec
TRUNCATE TABLE mysql_numeric_types;

/* String types */

-- name: InsertMysqlStringTypes :exec
INSERT INTO mysql_string_types 
(
    c_char,
    c_nchar,
    c_national_char,
    c_varchar,
    c_tinytext,
    c_mediumtext,
    c_text,
    c_longtext, 
    c_json,
    c_json_string_override,
    c_enum,
    c_set
) 
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: InsertMysqlStringTypesBatch :copyfrom
INSERT INTO mysql_string_types 
(
    c_char,
    c_nchar,
    c_national_char,
    c_varchar,
    c_tinytext,
    c_mediumtext,
    c_text,
    c_longtext, 
    c_json,
    c_json_string_override,
    c_enum,
    c_set
) 
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: GetMysqlStringTypes :one
SELECT * FROM mysql_string_types LIMIT 1;

-- name: GetMysqlStringTypesCnt :one
SELECT
    COUNT(*) AS cnt,
    c_char,
    c_nchar,
    c_national_char,
    c_varchar,
    c_tinytext,
    c_mediumtext,
    c_text,
    c_longtext,
    c_json,
    c_json_string_override,
    c_enum,
    c_set
FROM mysql_string_types
GROUP BY
    c_char,
    c_nchar,
    c_national_char,
    c_varchar,
    c_tinytext,
    c_mediumtext,
    c_text,
    c_longtext,
    c_json,
    c_json_string_override,
    c_enum,
    c_set
LIMIT 1;

-- name: TruncateMysqlStringTypes :exec
TRUNCATE TABLE mysql_string_types;

/* Datetime types */

-- name: InsertMysqlDatetimeTypes :exec
INSERT INTO mysql_datetime_types 
(
    c_year,
    c_date,
    c_datetime,
    c_timestamp,
    c_time
) 
VALUES (?, ?, ?, ?, ?);

-- name: InsertMysqlDatetimeTypesBatch :copyfrom
INSERT INTO mysql_datetime_types 
(
    c_year,
    c_date,
    c_datetime,
    c_timestamp,
    c_time
) 
VALUES (?, ?, ?, ?, ?);

-- name: GetMysqlDatetimeTypes :one
SELECT * FROM mysql_datetime_types LIMIT 1;

-- name: GetMysqlDatetimeTypesCnt :one
SELECT
    COUNT(*) AS cnt,
    c_year,
    c_date,
    c_datetime,
    c_timestamp,
    c_time
FROM mysql_datetime_types
GROUP BY
    c_year,
    c_date,
    c_datetime,
    c_timestamp,
    c_time
LIMIT 1;

-- name: TruncateMysqlDatetimeTypes :exec
TRUNCATE TABLE mysql_datetime_types;

/* Binary types */

-- name: InsertMysqlBinaryTypes :exec
INSERT INTO mysql_binary_types 
(
    c_bit,
    c_binary, 
    c_varbinary, 
    c_tinyblob, 
    c_blob, 
    c_mediumblob, 
    c_longblob
) 
VALUES (?, ?, ?, ?, ?, ?, ?);

-- name: InsertMysqlBinaryTypesBatch :copyfrom
INSERT INTO mysql_binary_types 
(
    c_bit,
    c_binary, 
    c_varbinary, 
    c_tinyblob, 
    c_blob, 
    c_mediumblob, 
    c_longblob
) 
VALUES (?, ?, ?, ?, ?, ?, ?);

-- name: GetMysqlBinaryTypes :one
SELECT * FROM mysql_binary_types LIMIT 1;

-- name: GetMysqlBinaryTypesCnt :one
SELECT
    COUNT(*) AS cnt,
    c_bit,
    c_binary,
    c_varbinary,
    c_tinyblob,
    c_blob,
    c_mediumblob,
    c_longblob
FROM mysql_binary_types
GROUP BY
    c_bit,
    c_binary,
    c_varbinary,
    c_tinyblob,
    c_blob,
    c_mediumblob,
    c_longblob
LIMIT 1;

-- name: TruncateMysqlBinaryTypes :exec
TRUNCATE TABLE mysql_binary_types;

/* Functions */

-- name: GetMysqlFunctions :one
SELECT
    MAX(c_int) AS max_int,
    MAX(c_varchar) AS max_varchar,
    MAX(c_timestamp) AS max_timestamp
FROM mysql_numeric_types
CROSS JOIN mysql_string_types
CROSS JOIN mysql_datetime_types;
