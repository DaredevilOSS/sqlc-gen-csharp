CREATE TABLE authors (
  id      BIGINT    PRIMARY KEY AUTO_INCREMENT,
  name    TEXT      NOT NULL,
  bio     TEXT
);

CREATE TABLE books (
  id            BIGINT      PRIMARY KEY AUTO_INCREMENT,
  name          TEXT        NOT NULL,
  author_id     BIGINT      NOT NULL,
  description   TEXT,
  FOREIGN KEY (author_id) REFERENCES authors (id) ON DELETE CASCADE
);

CREATE TABLE mysql_types (
  /* Boolean data types */
  c_bit BIT,
  c_tinyint TINYINT,
  c_bool BOOL,
  c_boolean BOOLEAN,

  /* Integer data types */
  c_smallint SMALLINT,
  c_mediumint MEDIUMINT,
  c_int INT,
  c_year YEAR,
  c_integer INTEGER,
  c_bigint BIGINT,

  /* Float data types */
  c_decimal DECIMAL(2,1),
  c_dec DEC(2,1),
  c_numeric NUMERIC(2,1),
  c_fixed FIXED(2,1),
  c_float FLOAT,
  c_double DOUBLE,
  c_double_precision DOUBLE PRECISION,

  /* Datetime data types */
  c_date DATE,
  c_time TIME,
  c_datetime DATETIME,
  c_timestamp TIMESTAMP,

  /* String data types */  
  c_char CHAR,
  c_nchar NCHAR,
  c_national_char NATIONAL CHAR,
  c_varchar VARCHAR(10),
  c_tinytext TINYTEXT,
  c_mediumtext MEDIUMTEXT,
  c_text TEXT,
  c_longtext LONGTEXT,
    
  /* Binary data types */
  c_binary BINARY,
  c_varbinary VARBINARY(10),
  c_tinyblob TINYBLOB,
  c_blob BLOB,
  c_mediumblob MEDIUMBLOB,
  c_longblob LONGBLOB,
  /* c_enum ENUM('a', 'b', 'c'), */
  /* c_set SET('a', 'b', 'c'), */

  c_json JSON
);
