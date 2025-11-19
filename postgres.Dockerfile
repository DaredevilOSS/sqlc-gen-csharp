FROM postgres:16.2

COPY examples/config/postgresql/types/schema.sql types_schema.sql
COPY examples/config/postgresql/authors/schema.sql authors_schema.sql
COPY examples/config/postgresql/benchmark/schema.sql benchmark_schema.sql

RUN (cat types_schema.sql && echo && cat authors_schema.sql && echo && cat benchmark_schema.sql) > /docker-entrypoint-initdb.d/schema.sql
