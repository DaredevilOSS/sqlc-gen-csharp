-- name: GetPostgresFunctions :one
SELECT
    MAX(c_integer) AS max_integer,
    MAX(c_varchar) AS max_varchar,
    MAX(c_timestamp) AS max_timestamp
FROM postgres_datetime_types
CROSS JOIN postgres_numeric_types
CROSS JOIN postgres_string_types;

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

/* Bit types */

-- name: InsertPostgresBitTypes :exec
INSERT INTO postgres_bit_types
(
    c_bit,
    c_bit_single,
    c_varbit,
    c_bit_varying,
    c_bit_array,
    c_varbit_array
)
VALUES ($1, $2, $3, $4, $5, $6);

-- name: GetPostgresBitTypes :one
SELECT * FROM postgres_bit_types LIMIT 1;

-- name: TruncatePostgresBitTypes :exec
TRUNCATE TABLE postgres_bit_types;

-- name: InsertPostgresBitTypesBatch :copyfrom
INSERT INTO postgres_bit_types
(
    c_bit,
    c_bit_single,
    c_varbit,
    c_bit_varying
) VALUES ($1, $2, $3, $4);

-- name: GetPostgresBitTypesCnt :one
SELECT
    c_bit,
    c_bit_single,
    c_varbit,
    c_bit_varying,
    COUNT(*) AS cnt
FROM postgres_bit_types
GROUP BY
    c_bit,
    c_bit_single,
    c_varbit,
    c_bit_varying
LIMIT 1;

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

/* System types */

-- name: InsertPostgresSystemTypes :exec
INSERT INTO postgres_system_types
(
    c_oid,
    c_xid,
    c_cid,
    c_xid8,
    c_tid,
    c_pg_lsn
) VALUES (
    sqlc.narg('c_oid'),
    sqlc.narg('c_xid'),
    sqlc.narg('c_cid'),
    sqlc.narg('c_xid8'),
    sqlc.narg('c_tid'),
    sqlc.narg('c_pg_lsn')
);

-- name: InsertPostgresSystemTypesBatch :copyfrom
INSERT INTO postgres_system_types
(
    c_oid,
    c_xid,
    c_cid,
    c_xid8,
    c_tid,
    c_pg_lsn
) VALUES ($1, $2, $3, $4, $5, $6);

-- name: GetPostgresSystemTypes :one
SELECT * FROM postgres_system_types LIMIT 1;

-- name: TruncatePostgresSystemTypes :exec
TRUNCATE TABLE postgres_system_types;

-- name: GetPostgresSystemTypesCnt :one
SELECT
    c_oid,
    c_xid,
    c_cid,
    c_xid8,
    c_tid,
    c_pg_lsn,
    COUNT(*) AS cnt
FROM postgres_system_types
GROUP BY
    c_oid,
    c_xid,
    c_cid,
    c_xid8,
    c_tid,
    c_pg_lsn
LIMIT 1;

/* Extended string types */

-- name: InsertPostgresExtendedStringTypes :exec
INSERT INTO postgres_extended_string_types
(
    c_citext,
    c_ltree,
    c_lquery,
    c_ltxtquery,
    c_name
) VALUES (
    sqlc.narg('c_citext'),
    sqlc.narg('c_ltree')::ltree,
    sqlc.narg('c_lquery')::lquery,
    sqlc.narg('c_ltxtquery')::ltxtquery,
    sqlc.narg('c_name')
);

-- name: InsertPostgresExtendedStringTypesBatch :copyfrom
INSERT INTO postgres_extended_string_types
(
    c_citext,
    c_ltree,
    c_lquery,
    c_ltxtquery,
    c_name
) VALUES ($1, $2, $3, $4, $5);

-- name: GetPostgresExtendedStringTypes :one
SELECT * FROM postgres_extended_string_types LIMIT 1;

-- name: TruncatePostgresExtendedStringTypes :exec
TRUNCATE TABLE postgres_extended_string_types;

-- name: GetPostgresExtendedStringTypesCnt :one
SELECT
    c_citext,
    c_ltree,
    c_name,
    COUNT(*) AS cnt
FROM postgres_extended_string_types
GROUP BY
    c_citext,
    c_ltree,
    c_name
LIMIT 1;

/* Range types */

-- name: InsertPostgresRangeTypes :exec
INSERT INTO postgres_range_types
(
    c_int4range,
    c_int8range,
    c_numrange,
    c_daterange,
    c_tsrange,
    c_tstzrange,
    c_int4range_array
) VALUES (
    sqlc.narg('c_int4range')::int4range,
    sqlc.narg('c_int8range')::int8range,
    sqlc.narg('c_numrange')::numrange,
    sqlc.narg('c_daterange')::daterange,
    sqlc.narg('c_tsrange')::tsrange,
    sqlc.narg('c_tstzrange')::tstzrange,
    sqlc.narg('c_int4range_array')::int4range[]
);

-- name: InsertPostgresRangeTypesBatch :copyfrom
INSERT INTO postgres_range_types
(
    c_int4range,
    c_int8range,
    c_numrange,
    c_daterange,
    c_tsrange,
    c_tstzrange,
    c_int4range_array
) VALUES ($1, $2, $3, $4, $5, $6, $7);

-- name: GetPostgresRangeTypes :one
SELECT * FROM postgres_range_types LIMIT 1;

-- name: TruncatePostgresRangeTypes :exec
TRUNCATE TABLE postgres_range_types;

-- name: GetPostgresRangeTypesCnt :one
SELECT
    c_int4range,
    c_int8range,
    c_numrange,
    c_daterange,
    c_tsrange,
    c_tstzrange,
    c_int4range_array,
    COUNT(*) AS cnt
FROM postgres_range_types
GROUP BY
    c_int4range,
    c_int8range,
    c_numrange,
    c_daterange,
    c_tsrange,
    c_tstzrange,
    c_int4range_array
LIMIT 1;

-- name: InsertPostgresMultirangeTypes :exec
INSERT INTO postgres_multirange_types
(
    c_int4multirange,
    c_int8multirange,
    c_nummultirange,
    c_datemultirange,
    c_tsmultirange,
    c_tstzmultirange
) VALUES (
    sqlc.narg('c_int4multirange')::int4multirange,
    sqlc.narg('c_int8multirange')::int8multirange,
    sqlc.narg('c_nummultirange')::nummultirange,
    sqlc.narg('c_datemultirange')::datemultirange,
    sqlc.narg('c_tsmultirange')::tsmultirange,
    sqlc.narg('c_tstzmultirange')::tstzmultirange
);

-- name: InsertPostgresMultirangeTypesBatch :copyfrom
INSERT INTO postgres_multirange_types
(
    c_int4multirange,
    c_int8multirange,
    c_nummultirange,
    c_datemultirange,
    c_tsmultirange,
    c_tstzmultirange
) VALUES ($1, $2, $3, $4, $5, $6);

-- name: GetPostgresMultirangeTypes :one
SELECT * FROM postgres_multirange_types LIMIT 1;

-- name: TruncatePostgresMultirangeTypes :exec
TRUNCATE TABLE postgres_multirange_types;

-- name: GetPostgresMultirangeTypesCnt :one
SELECT
    c_int4multirange,
    c_int8multirange,
    c_nummultirange,
    c_datemultirange,
    c_tsmultirange,
    c_tstzmultirange,
    COUNT(*) AS cnt
FROM postgres_multirange_types
GROUP BY
    c_int4multirange,
    c_int8multirange,
    c_nummultirange,
    c_datemultirange,
    c_tsmultirange,
    c_tstzmultirange
LIMIT 1;

/* DateTime types */

-- name: InsertPostgresDateTimeTypes :exec
INSERT INTO postgres_datetime_types
(
    c_date,
    c_time,
    c_timestamp,
    c_timestamp_with_tz,
    c_interval,
    c_timestamp_noda_instant_override
) VALUES ($1, $2, $3, $4, $5, $6);

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

/* Special types */

-- name: InsertPostgresSpecialTypes :exec
INSERT INTO postgres_special_types
(
    c_json,
    c_json_string_override,
    c_jsonb,
    c_jsonpath,
    c_xml,
    c_xml_string_override,
    c_uuid,
    c_enum,
    c_qualified_enum
)
VALUES (
    sqlc.narg('c_json'), 
    sqlc.narg('c_json_string_override')::json, 
    sqlc.narg('c_jsonb'),
    sqlc.narg('c_jsonpath')::jsonpath,
    sqlc.narg('c_xml')::xml,
    sqlc.narg('c_xml_string_override')::xml,
    sqlc.narg('c_uuid'),
    sqlc.narg('c_enum')::c_enum,
    sqlc.narg('c_qualified_enum')::c_enum
);

-- name: InsertPostgresNotNullTypes :exec
INSERT INTO postgres_not_null_types
(
    c_enum_not_null
)
VALUES (
    sqlc.arg('c_enum_not_null')::c_enum
);

-- name: GetPostgresNotNullTypes :one
SELECT
    c_enum_not_null
FROM postgres_not_null_types 
LIMIT 1;

-- name: TruncatePostgresNotNullTypes :exec
TRUNCATE TABLE postgres_not_null_types;

-- name: GetPostgresSpecialTypes :one
SELECT
    c_json,
    c_json_string_override,
    c_jsonb,
    c_jsonpath,
    c_xml,
    c_xml_string_override,
    c_uuid,
    c_enum,
    c_qualified_enum
FROM postgres_special_types 
LIMIT 1;

-- name: TruncatePostgresSpecialTypes :exec
TRUNCATE TABLE postgres_special_types;

-- name: InsertPostgresSpecialTypesBatch :copyfrom
INSERT INTO postgres_special_types
(
    c_uuid,
    c_json,
    c_jsonb
)
VALUES (
    $1,
    $2,
    $3
);

-- name: GetPostgresSpecialTypesCnt :one
WITH grouped_json_types AS (
    SELECT
        c_uuid,
        c_json::text AS c_json,
        c_jsonb::text AS c_jsonb,
        COUNT(*) AS cnt
    FROM postgres_special_types
    GROUP BY
        c_uuid,
        c_json::text,
        c_jsonb::text
)

SELECT 
    c_uuid, 
    c_json::json AS c_json, 
    c_jsonb::jsonb AS c_jsonb, 
    cnt
FROM grouped_json_types
LIMIT 1;

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
INSERT INTO postgres_array_types (
    c_bytea,
    c_boolean_array,
    c_text_array,
    c_integer_array,
    c_decimal_array,
    c_timestamp_array
) 
VALUES (
    $1,
    $2,
    $3,
    $4,
    $5,
    $6
);

-- name: GetPostgresArrayTypesCnt :one
SELECT
    c_bytea,
    c_boolean_array,
    c_text_array,
    c_integer_array,
    c_decimal_array,
    c_timestamp_array,
    COUNT(*) AS cnt
FROM postgres_array_types
GROUP BY
    c_bytea,
    c_boolean_array,
    c_text_array,
    c_integer_array,
    c_decimal_array,
    c_timestamp_array
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

