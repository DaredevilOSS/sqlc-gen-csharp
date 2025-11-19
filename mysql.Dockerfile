FROM mysql:8.3.0

COPY examples/config/mysql/types/schema.sql types_schema.sql
COPY examples/config/mysql/authors/schema.sql authors_schema.sql

RUN (cat types_schema.sql && echo && cat authors_schema.sql) > /docker-entrypoint-initdb.d/schema.sql
