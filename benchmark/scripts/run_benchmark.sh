#!/usr/bin/env bash

set -ex

if [ "$GITHUB_ACTIONS" = "true" ]; then
    echo "Running in Github Actions"
    dotnet build ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -c Release
    cd benchmark && dotnet run -c Release --no-build --project ./BenchmarkRunner/BenchmarkRunner.csproj
else
    echo "Running in local"
    
    destroy() { docker-compose down --volumes; }
    trap destroy EXIT
    
    docker-compose up --build --detach --force-recreate --remove-orphans --wait
    dotnet build ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -c Release
    
    cd benchmark && dotnet run -c Release --no-build --project ./BenchmarkRunner/BenchmarkRunner.csproj
fi