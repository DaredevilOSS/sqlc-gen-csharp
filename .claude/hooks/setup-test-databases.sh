#!/usr/bin/env bash
# Provides the PostgreSQL and MySQL servers that the end-to-end tests expect (normally docker-compose,
# but cloud sessions have no Docker daemon). Idempotent; safe to run manually. Needs root (apt, services).
# Settings come from .env; schemas are loaded exactly like postgres.Dockerfile / mysql.Dockerfile do.
set -euo pipefail

ROOT="${CLAUDE_PROJECT_DIR:-$(cd "$(dirname "$0")/../.." && pwd)}"
# shellcheck source=/dev/null
source "$ROOT/.env"
export DEBIAN_FRONTEND=noninteractive

PG_VERSION="$(ls /usr/lib/postgresql 2>/dev/null | sort -n | tail -n1 || true)"

LOCK="${TMPDIR:-/tmp}/test-databases.lock"
exec 9>"$LOCK"
flock 9

# --- install server packages when missing -----------------------------------------
missing=()
[ -n "$PG_VERSION" ] || missing+=(postgresql)
command -v mysqld > /dev/null || missing+=(mysql-server)
if [ ${#missing[@]} -gt 0 ]; then
  apt-get update -qq
  apt-get install -y -qq --no-install-recommends "${missing[@]}"
  PG_VERSION="$(ls /usr/lib/postgresql | sort -n | tail -n1)"
fi

schema() { # <db> -> concatenated schema, same order as the Dockerfiles
  local db="$1"
  for f in "examples/config/$db/types/schema.sql" "examples/config/$db/authors/schema.sql" "benchmark/config/$db/schema.sql"; do
    cat "$ROOT/$f"; echo
  done
}

# --- PostgreSQL --------------------------------------------------------------------
pg_lsclusters -h | grep -q online || pg_ctlcluster "$PG_VERSION" main start
until pg_isready -q; do sleep 1; done
runuser -u postgres -- psql -qtAc "ALTER USER ${POSTGRES_USER} PASSWORD '${POSTGRES_PASSWORD}'"
runuser -u postgres -- psql -qtAc "SELECT 1 FROM pg_database WHERE datname='${TESTS_DB}'" | grep -q 1 \
  || runuser -u postgres -- createdb "${TESTS_DB}"
if [ "$(runuser -u postgres -- psql -d "${TESTS_DB}" -qtAc "SELECT count(*) FROM information_schema.tables WHERE table_schema='public'")" = "0" ]; then
  schema postgresql | runuser -u postgres -- psql -q -v ON_ERROR_STOP=1 -d "${TESTS_DB}"
fi

# --- MySQL -------------------------------------------------------------------------
cat > /etc/mysql/conf.d/sqlc-gen-csharp.cnf <<'CNF'
[mysqld]
local-infile=1
bind-address=127.0.0.1
CNF
mkdir -p /var/run/mysqld && chown mysql:mysql /var/run/mysqld
mysqladmin ping --silent 2>/dev/null || service mysql start > /dev/null
until mysqladmin ping --silent 2>/dev/null; do sleep 1; done
mysql -e "ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY ''; SET GLOBAL local_infile=1; CREATE DATABASE IF NOT EXISTS \`${TESTS_DB}\`" 2>/dev/null \
  || mysql -uroot -e "SET GLOBAL local_infile=1; CREATE DATABASE IF NOT EXISTS \`${TESTS_DB}\`"
if [ "$(mysql -uroot -N -e "SELECT count(*) FROM information_schema.tables WHERE table_schema='${TESTS_DB}'")" = "0" ]; then
  schema mysql | mysql -uroot "${TESTS_DB}"
fi

echo "test databases ready (postgres :5432, mysql :3306, db=${TESTS_DB})"
