#!/usr/bin/env bash

set -ex

if [ "$GITHUB_ACTIONS" = "true" ]; then
    echo "Running in Github Actions"
    dotnet build ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -c Release
    dotnet run -c Release --no-build --project ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -- --sqlite
else
    echo "Running in local"
    
    destroy() { docker-compose down --volumes; }
    trap destroy EXIT
    
    # Compute hash of schema files to bust cache when they change
    SCHEMA_HASH=$(cat examples/config/postgresql/types/schema.sql examples/config/postgresql/authors/schema.sql examples/config/postgresql/benchmark/schema.sql | shasum -a 256 | cut -d' ' -f1)
    
    # Build with schema hash as build arg to ensure cache invalidation
    docker-compose up --build --detach --force-recreate --remove-orphans --wait
    dotnet build ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -c Release
    dotnet run -c Release --no-build --project ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -- --sqlite
fi