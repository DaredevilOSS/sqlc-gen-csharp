-- name: InsertSqliteTypes :exec
INSERT INTO types_sqlite 
(
    c_integer,
    c_real,
    c_text,
    c_blob,
    c_text_datetime_override,
    c_integer_datetime_override
) 
VALUES (?, ?, ?, ?, ?, ?);

-- name: InsertSqliteTypesBatch :copyfrom
INSERT INTO types_sqlite (c_integer, c_real, c_text) VALUES (?, ?, ?);

-- name: GetSqliteTypes :one
SELECT
    c_integer,
    c_real,
    c_text,
    c_blob,
    c_text_datetime_override,
    datetime(c_integer_datetime_override, 'unixepoch')
        AS c_integer_datetime_override
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
