-- name: InsertSqliteTypes :exec
INSERT INTO types_sqlite 
(
    c_integer,
    c_real,
    c_text,
    c_blob,
    c_text_datetime_override,
    c_integer_datetime_override,
    c_text_noda_instant_override,
    c_integer_noda_instant_override,
    c_text_bool_override,
    c_integer_bool_override
) 
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: InsertSqliteTypesBatch :copyfrom
INSERT INTO types_sqlite (c_integer, c_real, c_text) VALUES (?, ?, ?);

-- name: GetSqliteTypes :one
SELECT
    c_integer,
    c_real,
    c_text,
    c_blob,
    c_text_datetime_override,
    c_integer_datetime_override,
    c_text_noda_instant_override,
    c_integer_noda_instant_override,
    c_text_bool_override,
    c_integer_bool_override
FROM types_sqlite
LIMIT 1;

-- name: InsertSqliteNumericTypes :exec
INSERT INTO types_sqlite
(
    c_int,
    c_tinyint,
    c_smallint,
    c_mediumint,
    c_int2,
    c_bigint,
    c_unsigned_big_int,
    c_int8,
    c_double,
    c_double_precision,
    c_float,
    c_numeric,
    c_decimal
)
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: GetSqliteNumericTypes :one
SELECT
    c_int,
    c_tinyint,
    c_smallint,
    c_mediumint,
    c_int2,
    c_bigint,
    c_unsigned_big_int,
    c_int8,
    c_double,
    c_double_precision,
    c_float,
    c_numeric,
    c_decimal
FROM types_sqlite
LIMIT 1;

-- name: InsertSqliteStringTypes :exec
INSERT INTO types_sqlite
(
    c_character,
    c_varchar,
    c_varying_character,
    c_nchar,
    c_native_character,
    c_nvarchar,
    c_clob
)
VALUES (?, ?, ?, ?, ?, ?, ?);

-- name: GetSqliteStringTypes :one
SELECT
    c_character,
    c_varchar,
    c_varying_character,
    c_nchar,
    c_native_character,
    c_nvarchar,
    c_clob
FROM types_sqlite
LIMIT 1;

-- name: InsertSqliteMiscTypes :exec
INSERT INTO types_sqlite
(
    c_boolean,
    c_bool,
    c_date,
    c_datetime,
    c_timestamp,
    c_json,
    c_jsonb
)
VALUES (?, ?, ?, ?, ?, ?, ?);

-- name: GetSqliteMiscTypes :one
SELECT
    c_boolean,
    c_bool,
    c_date,
    c_datetime,
    c_timestamp,
    c_json,
    c_jsonb
FROM types_sqlite
LIMIT 1;

-- name: GetSqliteTypesCnt :one
SELECT
    c_integer,
    c_real,
    c_text,
    c_blob,
    count(*) AS cnt
FROM types_sqlite
GROUP BY c_integer, c_real, c_text, c_blob
LIMIT 1;

-- name: GetSqliteFunctions :one
SELECT
    max(c_integer) AS max_integer,
    max(c_real) AS max_real,
    max(c_text) AS max_text
FROM types_sqlite;

-- name: DeleteAllSqliteTypes :exec
DELETE FROM types_sqlite;
