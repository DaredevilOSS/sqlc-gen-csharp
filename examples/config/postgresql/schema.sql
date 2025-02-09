CREATE TABLE authors (
    id      BIGSERIAL PRIMARY KEY,
    name    TEXT      NOT NULL,
    bio     TEXT,
    created TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE books (
    id            BIGSERIAL   PRIMARY KEY,
    name          TEXT        NOT NULL,
    author_id     BIGINT      NOT NULL,
    description   TEXT,
    FOREIGN KEY (author_id) REFERENCES authors (id) ON DELETE CASCADE
);

CREATE TABLE postgres_types (
    id BIGSERIAL PRIMARY KEY,
    c_bit BIT(1),
    c_smallint SMALLINT,
    c_boolean BOOLEAN,
    c_integer INTEGER,
    c_bigint BIGINT,
    c_serial SERIAL,
    c_decimal DECIMAL(2,1),
    c_numeric NUMERIC(2,1),
    c_real REAL,
    c_double_precision DOUBLE PRECISION,
    
    /* Date and Time Data Types */
    c_date DATE,
    c_time TIME,
    c_timestamp TIMESTAMP,
    
    /* String Data Type Syntax */
    c_char CHAR,
    c_varchar VARCHAR(10),
    c_character_varying CHARACTER VARYING(10),
    c_bytea BYTEA,
    c_text TEXT,
    c_json JSON,

    /* Array Data Types */
    c_text_array TEXT[],
    c_integer_array INTEGER[]
);