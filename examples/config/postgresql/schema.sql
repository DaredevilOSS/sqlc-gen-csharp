CREATE TABLE authors (
    id BIGSERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    bio TEXT
);

CREATE EXTENSION "uuid-ossp";

CREATE TABLE books (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name TEXT NOT NULL,
    author_id BIGINT NOT NULL,
    description TEXT,
    FOREIGN KEY (author_id) REFERENCES authors (id) ON DELETE CASCADE
);

CREATE TABLE postgres_types (
    /* Numeric Data Types */
    c_boolean BOOLEAN,
    c_bit BIT(10),
    c_smallint SMALLINT,
    c_integer INTEGER,
    c_bigint BIGINT,
    c_decimal DECIMAL(10, 7),
    c_numeric NUMERIC(10, 7),
    c_real REAL,
    c_double_precision DOUBLE PRECISION,
    c_money MONEY,

    /* Date and Time Data Types */
    c_date DATE,
    c_time TIME,
    c_timestamp TIMESTAMP,
    c_timestamp_with_tz TIMESTAMP WITH TIME ZONE,
    c_interval INTERVAL,

    /* String Data Type Syntax */
    c_char CHAR,
    c_varchar VARCHAR(100),
    c_character_varying CHARACTER VARYING(100),
    c_bpchar BPCHAR(100),
    c_text TEXT,

    /* JSON Data Types */
    c_json JSON,
    c_json_string_override JSON,
    c_jsonb JSONB,
    c_jsonpath JSONPATH,

    /* Network Address Data Types */
    c_cidr CIDR,
    c_inet INET,
    c_macaddr MACADDR,
    c_macaddr8 MACADDR8,

    /* Special Data Types */
    c_uuid UUID,

    /* Array Data Types */
    c_bytea BYTEA,
    c_boolean_array BOOLEAN [],
    c_text_array TEXT [],
    c_integer_array INTEGER [],
    c_decimal_array DECIMAL(10, 7) [],
    c_date_array DATE [],
    c_timestamp_array TIMESTAMP []
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
