-- name: InsertMysqlTypes :exec
INSERT INTO mysql_types 
(
    c_bit,
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
    c_set,
    c_year,
    c_date,
    c_datetime,
    c_timestamp, 
    c_binary, c_varbinary, c_tinyblob, c_blob, c_mediumblob, c_longblob
) 
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: InsertMysqlTypesBatch :copyfrom
INSERT INTO mysql_types 
(
    c_bit,
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
    c_set,
    c_year,
    c_date,
    c_datetime,
    c_timestamp,
    c_binary, c_varbinary, c_tinyblob, c_blob, c_mediumblob, c_longblob
) 
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: GetMysqlTypes :one
SELECT * FROM mysql_types LIMIT 1;

-- name: GetMysqlTypesCnt :one
SELECT
    COUNT(*) AS cnt,
    c_bool,
    c_boolean,
    c_bit,
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
    c_set,
    c_year,
    c_date,
    c_datetime,
    c_timestamp,
    c_binary,
    c_varbinary,
    c_tinyblob,
    c_blob,
    c_mediumblob,
    c_longblob
FROM mysql_types
GROUP BY
    c_bool,
    c_boolean,
    c_bit,
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
    c_set,
    c_year,
    c_date,
    c_datetime,
    c_timestamp,
    c_binary, c_varbinary, c_tinyblob, c_blob, c_mediumblob, c_longblob
LIMIT 1;

-- name: GetMysqlFunctions :one
SELECT
    MAX(c_int) AS max_int,
    MAX(c_varchar) AS max_varchar,
    MAX(c_timestamp) AS max_timestamp
FROM mysql_types;

-- name: TruncateMysqlTypes :exec
TRUNCATE TABLE mysql_types;
