/* Basic types */

-- name: InsertMysqlTypes :exec
INSERT INTO mysql_types 
(
    c_bool,
    c_boolean,
    c_tinyint,
    c_smallint,
    c_mediumint,
    c_int,
    c_integer,
    c_bigint, 
    c_decimal, c_dec, c_numeric, c_fixed, c_float, c_double, c_double_precision, 
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
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: InsertMysqlTypesBatch :copyfrom
INSERT INTO mysql_types 
(
    c_bool,
    c_boolean,
    c_tinyint,
    c_smallint,
    c_mediumint,
    c_int,
    c_integer,
    c_bigint, 
    c_float, c_numeric, c_decimal, c_dec, c_fixed, c_double, c_double_precision, 
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
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: GetMysqlTypes :one
SELECT * FROM mysql_types LIMIT 1;

-- name: GetMysqlTypesCnt :one
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
    c_double_precision,
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
FROM mysql_types
GROUP BY
    c_bool,
    c_boolean,
    c_tinyint,
    c_smallint,
    c_mediumint,
    c_int,
    c_integer,
    c_bigint,
    c_float, c_numeric, c_decimal, c_dec, c_fixed, c_double, c_double_precision,
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

-- name: TruncateMysqlTypes :exec
TRUNCATE TABLE mysql_types;

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
FROM mysql_types
CROSS JOIN mysql_datetime_types;
