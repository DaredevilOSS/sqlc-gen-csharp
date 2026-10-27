CREATE TABLE mssql_numeric_types (
    c_bit BIT,
    c_tinyint TINYINT,
    c_smallint SMALLINT,
    c_int INT,
    c_bigint BIGINT,
    c_real REAL,
    c_float FLOAT,
    c_decimal DECIMAL(18, 4),
    c_numeric NUMERIC(18, 4),
    c_money MONEY,
    c_smallmoney SMALLMONEY
);

CREATE TABLE mssql_string_types (
    c_char CHAR(10),
    c_nchar NCHAR(10),
    c_varchar VARCHAR(100),
    c_nvarchar NVARCHAR(100),
    c_varchar_max VARCHAR(MAX),
    c_nvarchar_max NVARCHAR(MAX)
);

CREATE TABLE mssql_datetime_types (
    c_date DATE,
    c_datetime DATETIME,
    c_datetime2 DATETIME2(7),
    c_smalldatetime SMALLDATETIME,
    c_datetimeoffset DATETIMEOFFSET,
    c_time TIME
);

CREATE TABLE mssql_binary_types (
    c_binary BINARY(3),
    c_varbinary VARBINARY(10),
    c_varbinary_max VARBINARY(MAX),
    c_uniqueidentifier UNIQUEIDENTIFIER
);

CREATE TABLE mssql_spatial_types (
    c_geography GEOGRAPHY NULL,
    c_geometry GEOMETRY NULL,
    c_hierarchyid HIERARCHYID NULL
);

CREATE TABLE mssql_noda_types (
    c_datetime2_instant DATETIME2 NULL,
    c_datetimeoffset_instant DATETIMEOFFSET NULL
);
