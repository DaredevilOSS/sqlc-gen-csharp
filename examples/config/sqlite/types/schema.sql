CREATE TABLE types_sqlite (
    c_integer                   INTEGER,
    c_real                      REAL,
    c_text                      TEXT,
    c_blob                      BLOB,
    c_text_datetime_override    TEXT DEFAULT (datetime('now')),
    c_integer_datetime_override INTEGER
);