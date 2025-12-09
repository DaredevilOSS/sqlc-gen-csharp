#!/usr/bin/env bash

set -ex

database_to_benchmark=$1

if [ "$GITHUB_ACTIONS" = "true" ]; then
    echo "Running in Github Actions"
    dotnet run -c Release --project ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -- --database $database_to_benchmark
else
    destroy() { docker-compose down --volumes; }
    trap destroy EXIT
    
    echo "Running in local"
    docker-compose up --build --detach --force-recreate --remove-orphans --wait
    dotnet run -c Release --project ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -- --database $database_to_benchmark
fi