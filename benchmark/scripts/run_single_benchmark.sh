#!/usr/bin/env bash

set -ex

database_to_benchmark=$1
type_to_benchmark=$2

function dotnet_run() {
    dotnet run -c Release --project ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -- \
        --database $database_to_benchmark \
        --type $type_to_benchmark
}

function docker_compose_up() {
    service_name=$1
    docker-compose up --build --detach --force-recreate --remove-orphans --wait $service_name
}

# Adjust the SQLite connection string to use absolute path
function adjust_sqlite_connection_string() {
    if [[ "$OSTYPE" == "darwin"* ]]; then
        sed -i '' "s|Data Source=\([^;]*\.db\);|Data Source=$(pwd)/\1;|" .env
    else
        sed -i "s|Data Source=\([^;]*\.db\);|Data Source=$(pwd)/\1;|" .env
    fi
}

function delete_current_sqlite_db() {
    rm -f $(pwd)/.sqlite/*.db
}

function docker_destroy() { 
    docker-compose down --volumes
}

function copy_original_env() { 
    cp .env.bak .env 
}

if [ -n "$GITHUB_ACTIONS" ]; then
    if [ "$database_to_benchmark" = "mysql" ]; then
        trap docker_destroy EXIT
        docker_compose_up mysqldb
    elif [ "$database_to_benchmark" = "postgresql" ]; then
        trap docker_destroy EXIT
        docker_compose_up postgresdb
    fi
fi

if [ "$database_to_benchmark" = "sqlite" ]; then
    cp .env .env.bak
    trap copy_original_env EXIT
    adjust_sqlite_connection_string
    delete_current_sqlite_db
fi

dotnet_run