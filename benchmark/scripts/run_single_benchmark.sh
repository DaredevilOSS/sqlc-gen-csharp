#!/usr/bin/env bash

set -ex

database_to_benchmark=$1
type_to_benchmark=$2

function dotnet_run() {
    dotnet run -c Release --project ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -- \
        --database $database_to_benchmark \
        --type $type_to_benchmark
}

if [ "$GITHUB_ACTIONS" = "true" ]; then
    echo "Running in Github Actions"
    dotnet_run
else
    destroy() { docker-compose down --volumes; }
    trap destroy EXIT
    
    echo "Running in local"
    if [ "$database_to_benchmark" = "mysql" ]; then
        docker-compose up --build --detach --force-recreate --remove-orphans --wait mysqldb
    elif [ "$database_to_benchmark" = "postgresql" ]; then
        docker-compose up --build --detach --force-recreate --remove-orphans --wait postgresdb
    fi
    dotnet_run
fi