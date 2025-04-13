CREATE TABLE authors (
  id      BIGINT    PRIMARY KEY AUTO_INCREMENT,
  name    TEXT      NOT NULL,
  bio     TEXT
);

CREATE TABLE books (
  id            BIGINT      PRIMARY KEY AUTO_INCREMENT,
  name          TEXT        NOT NULL,
  author_id     BIGINT      NOT NULL,
  description   TEXT
);

CREATE TABLE mysql_types (
  /* Boolean data types - TINYINT(1) synonyms */
  c_bool        BOOL,
  c_boolean     BOOLEAN,

  /* Integer data types */
  c_tinyint     TINYINT(3),
  c_smallint    SMALLINT,
  c_mediumint   MEDIUMINT,
  c_int         INT,
  c_integer     INTEGER,
  c_bigint      BIGINT,

  /* Float data types */
  c_float            FLOAT,
  c_decimal          DECIMAL(10,7),
  c_dec              DEC(10,7),
  c_numeric          NUMERIC(10,7),
  c_fixed            FIXED(10,7),
  c_double           DOUBLE,
  c_double_precision DOUBLE PRECISION,

  /* Datetime data types */
  c_year        YEAR,
  c_date        DATE,
  c_time        TIME,
  c_datetime    DATETIME,
  c_timestamp   TIMESTAMP,

  /* String data types */  
  c_char            CHAR,
  c_nchar           NCHAR,
  c_national_char   NATIONAL CHAR,
  c_varchar         VARCHAR(100),
  c_tinytext        TINYTEXT,
  c_mediumtext      MEDIUMTEXT,
  c_text            TEXT,
  c_longtext        LONGTEXT,
  c_enum            ENUM ('small', 'medium', 'big'),
    
  /* Binary data types */
  c_bit         BIT(8),
  c_binary      BINARY(3),
  c_varbinary   VARBINARY(10),
  c_tinyblob    TINYBLOB,
  c_blob        BLOB,
  c_mediumblob  MEDIUMBLOB,
  c_longblob    LONGBLOB
);

CREATE SCHEMA extended; 

CREATE TABLE extended.biographies (
  author_name   VARCHAR(100),
  name          VARCHAR(100),
  bio_type      ENUM ('Autobiography', 'Biography', 'Memoir'),
  PRIMARY KEY (author_name, name)
);
