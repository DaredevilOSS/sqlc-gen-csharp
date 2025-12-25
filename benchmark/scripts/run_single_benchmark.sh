#!/usr/bin/env bash

set -e

working_directory=$(git rev-parse --show-toplevel)/benchmark
cd $working_directory

database_to_benchmark=$1
type_to_benchmark=$2

function docker_compose_up() {
    service_name=$1
    docker-compose up --build --detach --force-recreate --remove-orphans --wait $service_name
}

# Adjust the SQLite connection string to use absolute path
function adjust_sqlite_connection_string() {
    sed_pattern="s|Data Source=\([^;]*\.db\);|Data Source=$(pwd)/\1;|"
    if [[ "$OSTYPE" == "darwin"* ]]; then
        sed -i '' "$sed_pattern" .env
    else
        sed -i "$sed_pattern" .env
    fi
}

# Github Actions handles the Docker Compose setup, this is only needed when running locally
if [[ -z "$GITHUB_ACTIONS" ]]; then
    docker_destroy() { docker-compose down --volumes; }

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
    copy_original_env() { cp .env.bak .env; }
    trap copy_original_env EXIT

    adjust_sqlite_connection_string
    rm -f $(pwd)/.sqlite/*.db
fi

dotnet run -c Release --project ./BenchmarkRunner/BenchmarkRunner.csproj -- \
    --database $database_to_benchmark \
    --type $type_to_benchmark
