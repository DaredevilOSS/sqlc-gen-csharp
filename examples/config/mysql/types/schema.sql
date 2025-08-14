CREATE TABLE mysql_types (
  /* Numeric data types */
  c_bool             BOOL,
  c_boolean          BOOLEAN,
  c_tinyint          TINYINT(3),
  c_smallint         SMALLINT,
  c_mediumint        MEDIUMINT,
  c_int              INT,
  c_integer          INTEGER,
  c_bigint           BIGINT,
  c_float            FLOAT,
  c_decimal          DECIMAL(10,7),
  c_dec              DEC(10,7),
  c_numeric          NUMERIC(10,7),
  c_fixed            FIXED(10,7),
  c_double           DOUBLE,
  c_double_precision DOUBLE PRECISION,

  /* String data types */  
  c_char                 CHAR,
  c_nchar                NCHAR,
  c_national_char        NATIONAL CHAR,
  c_varchar              VARCHAR(100),
  c_tinytext             TINYTEXT,
  c_mediumtext           MEDIUMTEXT,
  c_text                 TEXT,
  c_longtext             LONGTEXT,
  c_json                 JSON,
  c_json_string_override JSON,  

  /* Pre-defined types */
  c_enum         ENUM ('small', 'medium', 'big'),
  c_set          SET ('tea', 'coffee', 'milk')
);

CREATE TABLE mysql_datetime_types (
  c_year        YEAR,
  c_date        DATE,
  c_time        TIME,
  c_datetime    DATETIME,
  c_timestamp   TIMESTAMP
);

CREATE TABLE mysql_binary_types (
  c_bit         BIT(8),
  c_binary      BINARY(3),
  c_varbinary   VARBINARY(10),
  c_tinyblob    TINYBLOB,
  c_blob        BLOB,
  c_mediumblob  MEDIUMBLOB,
  c_longblob    LONGBLOB
);