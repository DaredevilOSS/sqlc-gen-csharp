#!/usr/bin/env bash

set -ex

destroy() { docker-compose down --volumes; }
trap destroy EXIT
docker-compose up --build --detach --force-recreate --remove-orphans --wait

./benchmark/scripts/run_benchmark_for_db.sh mysql
./benchmark/scripts/run_benchmark_for_db.sh postgresql
./benchmark/scripts/run_benchmark_for_db.sh sqlite
