CREATE EXTENSION "uuid-ossp";

CREATE TYPE c_enum AS ENUM ('small', 'medium', 'big');

CREATE TABLE postgres_types (
    /* Special Data Types */
    c_uuid UUID,
    c_enum c_enum
);

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

CREATE TABLE postgres_string_types (
    c_char              CHAR,
    c_varchar           VARCHAR(100),
    c_character_varying CHARACTER VARYING(100),
    c_bpchar            BPCHAR(100),
    c_text              TEXT
);

CREATE TABLE postgres_datetime_types (
    c_date              DATE,
    c_time              TIME,
    c_timestamp         TIMESTAMP,
    c_timestamp_with_tz TIMESTAMP WITH TIME ZONE,
    c_interval          INTERVAL
);

CREATE TABLE postgres_network_types (
    c_cidr      CIDR,
    c_inet      INET,
    c_macaddr   MACADDR,
    c_macaddr8  MACADDR8
);

CREATE EXTENSION "pg_trgm";
CREATE EXTENSION "btree_gin";

CREATE INDEX postgres_txt_idx ON postgres_string_types USING GIN (c_text);

CREATE TABLE postgres_unstructured_types (
    c_json                 JSON,
    c_json_string_override JSON,
    c_jsonb                JSONB,
    c_jsonpath             JSONPATH,
    c_xml                  XML,
    c_xml_string_override  XML
);

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
    c_point POINT,
    c_line LINE,
    c_lseg LSEG,
    c_box BOX,
    c_path PATH,
    c_polygon POLYGON,
    c_circle CIRCLE
);