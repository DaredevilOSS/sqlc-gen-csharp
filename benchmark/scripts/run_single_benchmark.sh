#!/usr/bin/env bash

set -ex

database_to_benchmark=$1
type_to_benchmark=$2

function dotnet_run() {
    dotnet run -c Release --project ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -- \
        --database $database_to_benchmark \
        --type $type_to_benchmark
}

function docker-compose-up() {
    service_name=$1
    docker-compose up --build --detach --force-recreate --remove-orphans --wait $service_name
}

# Adjust the SQLite connection string to use absolute path
function adjust-sqlite-connection-string() {
    if [[ "$OSTYPE" == "darwin"* ]]; then
        sed -i '' "s|Data Source=\([^;]*\.db\);|Data Source=$(pwd)/.sqlite/\1;|" .env
    else
        sed -i "s|Data Source=\([^;]*\.db\);|Data Source=$(pwd)/.sqlite/\1;|" .env
    fi
}

function delete-current-sqlite-db() {
    rm -f $(pwd)/.sqlite/*.db
}


docker-destroy() { docker-compose down --volumes; }
copy-original-env() { cp .env.bak .env; }

if [ "$database_to_benchmark" = "mysql" ]; then
    trap docker-destroy EXIT
    docker-compose-up mysqldb
elif [ "$database_to_benchmark" = "postgresql" ]; then
    trap docker-destroy EXIT
    docker-compose-up postgresdb
elif [ "$database_to_benchmark" = "sqlite" ]; then
    cp .env .env.bak
    trap copy-original-env EXIT
    adjust-sqlite-connection-string
    delete-current-sqlite-db
fi

dotnet_run