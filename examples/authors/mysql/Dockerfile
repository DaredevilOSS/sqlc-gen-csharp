FROM mysql:8.3.0

ENV MYSQL_DATABASE=tests
ENV MYSQL_ALLOW_EMPTY_PASSWORD=true

WORKDIR /var/db
COPY . .
ADD schema.sql /docker-entrypoint-initdb.d
