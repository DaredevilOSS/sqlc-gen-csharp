/* Special types */

-- name: InsertPostgresTypes :exec
INSERT INTO postgres_types
(
    c_uuid,
    c_enum
)
VALUES (
    sqlc.narg('c_uuid'),
    sqlc.narg('c_enum')::c_enum
);

-- name: InsertPostgresTypesBatch :copyfrom
INSERT INTO postgres_types
(
    c_uuid
)
VALUES (
    $1
);

-- name: GetPostgresTypes :one
SELECT
    c_uuid,
    c_enum
FROM postgres_types 
LIMIT 1;

-- name: GetPostgresTypesCnt :one
SELECT
    c_uuid,
    COUNT(*) AS cnt
FROM postgres_types
GROUP BY
    c_uuid
LIMIT 1;

-- name: GetPostgresFunctions :one
SELECT
    MAX(c_integer) AS max_integer,
    MAX(c_varchar) AS max_varchar,
    MAX(c_timestamp) AS max_timestamp
FROM postgres_datetime_types
CROSS JOIN postgres_numeric_types
CROSS JOIN postgres_string_types;

-- name: TruncatePostgresTypes :exec
TRUNCATE TABLE postgres_types;

/* Numeric types */

-- name: InsertPostgresNumericTypes :exec
INSERT INTO postgres_numeric_types
(
    c_boolean,
    c_bit,
    c_smallint,
    c_integer,
    c_bigint,
    c_decimal,
    c_numeric,
    c_real,
    c_double_precision,
    c_money
)
VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10);

-- name: GetPostgresNumericTypes :one
SELECT * FROM postgres_numeric_types LIMIT 1;

-- name: TruncatePostgresNumericTypes :exec
TRUNCATE TABLE postgres_numeric_types;

-- name: GetPostgresNumericTypesCnt :one
SELECT
    c_boolean,
    c_bit,
    c_smallint,
    c_integer,
    c_bigint,
    c_decimal,
    c_numeric,
    c_real,
    c_double_precision,
    c_money,
    COUNT(*) AS cnt
FROM postgres_numeric_types
GROUP BY
    c_boolean,
    c_bit,
    c_smallint,
    c_integer,
    c_bigint,
    c_decimal,
    c_numeric,
    c_real,
    c_double_precision,
    c_money
LIMIT 1;

-- name: InsertPostgresNumericTypesBatch :copyfrom
INSERT INTO postgres_numeric_types
(
    c_boolean,
    c_bit,
    c_smallint,
    c_integer,
    c_bigint,
    c_decimal,
    c_numeric,
    c_real,
    c_double_precision,
    c_money
) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10);

/* String types */

-- name: InsertPostgresStringTypes :exec
INSERT INTO postgres_string_types
(
    c_char,
    c_varchar,
    c_character_varying,
    c_bpchar,
    c_text
)
VALUES ($1, $2, $3, $4, $5);

-- name: InsertPostgresStringTypesBatch :copyfrom
INSERT INTO postgres_string_types 
(
    c_char,
    c_varchar,
    c_character_varying,
    c_bpchar,
    c_text
) VALUES ($1, $2, $3, $4, $5);

-- name: GetPostgresStringTypes :one
SELECT * FROM postgres_string_types LIMIT 1;

-- name: TruncatePostgresStringTypes :exec
TRUNCATE TABLE postgres_string_types;

-- name: GetPostgresStringTypesCnt :one
SELECT
    c_char,
    c_varchar,
    c_character_varying,
    c_bpchar,
    c_text,
    COUNT(*) AS cnt
FROM postgres_string_types
GROUP BY
    c_char,
    c_varchar,
    c_character_varying,
    c_bpchar,
    c_text
LIMIT 1;

-- name: GetPostgresStringTypesTextSearch :one
WITH txt_query AS (
    SELECT 
        c_text, 
        to_tsquery('english', $1) AS query,
        to_tsvector('english', c_text) AS tsv
    FROM postgres_string_types 
    WHERE c_text @@ to_tsquery('english', $1)
)

SELECT txt_query.*, ts_rank(tsv, query) AS rnk
FROM txt_query
ORDER BY rnk DESC
LIMIT 1;

/* DateTime types */

-- name: InsertPostgresDateTimeTypes :exec
INSERT INTO postgres_datetime_types
(
    c_date,
    c_time,
    c_timestamp,
    c_timestamp_with_tz,
    c_interval
) VALUES ($1, $2, $3, $4, $5);

-- name: GetPostgresDateTimeTypes :one
SELECT * FROM postgres_datetime_types LIMIT 1;

-- name: TruncatePostgresDateTimeTypes :exec
TRUNCATE TABLE postgres_datetime_types;

-- name: GetPostgresDateTimeTypesCnt :one
SELECT
    c_date,
    c_time,
    c_timestamp,
    c_timestamp_with_tz,
    c_interval,
    COUNT(*) AS cnt
FROM postgres_datetime_types
GROUP BY
    c_date,
    c_time,
    c_timestamp,
    c_timestamp_with_tz,
    c_interval
LIMIT 1;

-- name: InsertPostgresDateTimeTypesBatch :copyfrom
INSERT INTO postgres_datetime_types
(
    c_date,
    c_time,
    c_timestamp,
    c_timestamp_with_tz,
    c_interval
) VALUES ($1, $2, $3, $4, $5);

/* Network types */

-- name: InsertPostgresNetworkTypes :exec
INSERT INTO postgres_network_types
(
    c_cidr,
    c_inet,
    c_macaddr,
    c_macaddr8
) VALUES (
    sqlc.narg('c_cidr'), 
    sqlc.narg('c_inet'), 
    sqlc.narg('c_macaddr'), 
    sqlc.narg('c_macaddr8')::macaddr8
);

-- name: GetPostgresNetworkTypes :one
SELECT
    c_cidr,
    c_inet,
    c_macaddr,
    c_macaddr8::TEXT AS c_macaddr8
FROM postgres_network_types
LIMIT 1;

-- name: TruncatePostgresNetworkTypes :exec
TRUNCATE TABLE postgres_network_types;

-- name: GetPostgresNetworkTypesCnt :one
SELECT
    c_cidr,
    c_inet,
    c_macaddr,
    COUNT(*) AS cnt
FROM postgres_network_types
GROUP BY
    c_cidr,
    c_inet,
    c_macaddr
LIMIT 1;

-- name: InsertPostgresNetworkTypesBatch :copyfrom
INSERT INTO postgres_network_types
(
    c_cidr,
    c_inet,
    c_macaddr
) VALUES ($1, $2, $3);

/* Unstructured types */

-- name: InsertPostgresUnstructuredTypes :exec
INSERT INTO postgres_unstructured_types
(
    c_json,
    c_json_string_override,
    c_jsonb,
    c_jsonpath,
    c_xml,
    c_xml_string_override
)
VALUES (
    sqlc.narg('c_json')::json, 
    sqlc.narg('c_json_string_override')::json, 
    sqlc.narg('c_jsonb')::jsonb,
    sqlc.narg('c_jsonpath')::jsonpath,
    sqlc.narg('c_xml')::xml,
    sqlc.narg('c_xml_string_override')::xml
);

-- name: GetPostgresUnstructuredTypes :one
SELECT
    c_json,
    c_json_string_override,
    c_jsonb,
    c_jsonpath,
    c_xml,
    c_xml_string_override
FROM postgres_unstructured_types 
LIMIT 1;

-- name: TruncatePostgresUnstructuredTypes :exec
TRUNCATE TABLE postgres_unstructured_types;

/* Array types */

-- name: InsertPostgresArrayTypes :exec
INSERT INTO postgres_array_types
(
    c_bytea,
    c_boolean_array,
    c_text_array,
    c_integer_array,
    c_decimal_array,
    c_date_array,
    c_timestamp_array
)
VALUES ($1, $2, $3, $4, $5, $6, $7);

-- name: GetPostgresArrayTypes :one
SELECT * FROM postgres_array_types LIMIT 1;

-- name: InsertPostgresArrayTypesBatch :copyfrom
INSERT INTO postgres_array_types (c_bytea) VALUES ($1);

-- name: GetPostgresArrayTypesCnt :one
SELECT
    c_bytea,
    COUNT(*) AS cnt
FROM postgres_array_types
GROUP BY
    c_bytea
LIMIT 1;

-- name: TruncatePostgresArrayTypes :exec
TRUNCATE TABLE postgres_array_types;


/* Geometric types */

-- name: InsertPostgresGeoTypes :exec
INSERT INTO postgres_geometric_types (
    c_point, 
    c_line, 
    c_lseg, 
    c_box, 
    c_path, 
    c_polygon, 
    c_circle
)
VALUES ($1, $2, $3, $4, $5, $6, $7);

-- name: InsertPostgresGeoTypesBatch :copyfrom
INSERT INTO postgres_geometric_types (
    c_point, 
    c_line, 
    c_lseg, 
    c_box, 
    c_path, 
    c_polygon, 
    c_circle
)
VALUES ($1, $2, $3, $4, $5, $6, $7);

-- name: GetPostgresGeoTypes :one
SELECT * FROM postgres_geometric_types LIMIT 1;

-- name: TruncatePostgresGeoTypes :exec
TRUNCATE TABLE postgres_geometric_types;
