FROM postgres:16.2

COPY types/schema.sql types_schema.sql
COPY authors/schema.sql authors_schema.sql

RUN (cat types_schema.sql && echo && cat authors_schema.sql) > /docker-entrypoint-initdb.d/schema.sql
