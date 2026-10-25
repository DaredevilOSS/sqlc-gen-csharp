CREATE TABLE postgres_numeric_types (
    c_boolean BOOLEAN,
    c_bit BIT(10),
    c_smallint SMALLINT,
    c_integer INTEGER,
    c_bigint BIGINT,
    c_decimal DECIMAL(10, 7),
    c_numeric NUMERIC(10, 7),
    c_real REAL,
    c_double_precision DOUBLE PRECISION,
    c_money MONEY
);

CREATE TABLE postgres_bit_types (
    c_bit               BIT(10),
    c_bit_single        BIT(1),
    c_varbit            VARBIT(10),
    c_bit_varying       BIT VARYING(10),
    c_bit_array         BIT(10) [],
    c_varbit_array      VARBIT(10) []
);

CREATE TABLE postgres_string_types (
    c_char CHAR,
    c_varchar VARCHAR(100),
    c_character_varying CHARACTER VARYING(100),
    c_bpchar BPCHAR(100),
    c_text TEXT
);

CREATE EXTENSION IF NOT EXISTS "citext";
CREATE EXTENSION IF NOT EXISTS "ltree";
CREATE EXTENSION IF NOT EXISTS "hstore";

CREATE TABLE postgres_hstore_types (
    c_hstore        HSTORE,
    c_hstore_array  HSTORE []
);

CREATE TABLE postgres_range_types (
    c_int4range         INT4RANGE,
    c_int8range         INT8RANGE,
    c_numrange          NUMRANGE,
    c_daterange         DATERANGE,
    c_tsrange           TSRANGE,
    c_tstzrange         TSTZRANGE,
    c_int4range_array   INT4RANGE []
);

CREATE TABLE postgres_multirange_types (
    c_int4multirange    INT4MULTIRANGE,
    c_int8multirange    INT8MULTIRANGE,
    c_nummultirange     NUMMULTIRANGE,
    c_datemultirange    DATEMULTIRANGE,
    c_tsmultirange      TSMULTIRANGE,
    c_tstzmultirange    TSTZMULTIRANGE
);

CREATE TABLE postgres_system_types (
    c_oid       OID,
    c_xid       XID,
    c_cid       CID,
    c_xid8      XID8,
    c_tid       TID,
    c_pg_lsn    PG_LSN
);

CREATE TABLE postgres_extended_string_types (
    c_citext       CITEXT,
    c_ltree        LTREE,
    c_lquery       LQUERY,
    c_ltxtquery    LTXTQUERY,
    c_name         NAME
);

CREATE TABLE postgres_datetime_types (
    c_date DATE,
    c_time TIME,
    c_timestamp TIMESTAMP,
    c_timestamp_with_tz TIMESTAMP WITH TIME ZONE,
    c_interval INTERVAL,
    c_timestamp_noda_instant_override TIMESTAMP
);

CREATE TABLE postgres_network_types (
    c_cidr CIDR,
    c_inet INET,
    c_macaddr MACADDR,
    c_macaddr8 MACADDR8
);

CREATE EXTENSION "pg_trgm";
CREATE EXTENSION "btree_gin";

CREATE INDEX postgres_txt_idx ON postgres_string_types USING GIN (c_text);

CREATE TABLE postgres_array_types (
    c_bytea             BYTEA,
    c_boolean_array     BOOLEAN [],
    c_text_array        TEXT [],
    c_integer_array     INTEGER [],
    c_decimal_array     DECIMAL(10, 7) [],
    c_date_array        DATE [],
    c_timestamp_array   TIMESTAMP []
);

CREATE TABLE postgres_geometric_types (
    c_point     POINT,
    c_line      LINE,
    c_lseg      LSEG,
    c_box       BOX,
    c_path      PATH,
    c_polygon   POLYGON,
    c_circle    CIRCLE
);

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TYPE c_enum AS ENUM ('small', 'medium', 'big');

CREATE TABLE postgres_special_types (
    c_uuid                 UUID,
    c_enum                 c_enum,
    c_qualified_enum       public.c_enum,
    c_json                 JSON,
    c_json_string_override JSON,
    c_jsonb                JSONB,
    c_jsonpath             JSONPATH,
    c_xml                  XML,
    c_xml_string_override  XML
);

CREATE TABLE postgres_not_null_types (
    c_enum_not_null        c_enum NOT NULL DEFAULT 'small'
);