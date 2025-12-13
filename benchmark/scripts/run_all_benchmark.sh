#!/usr/bin/env bash

set -ex

destroy() { docker-compose down --volumes; }
trap destroy EXIT
docker-compose up --build --detach --force-recreate --remove-orphans --wait

dotnet build ./benchmark/BenchmarkRunner/BenchmarkRunner.csproj -c Release

./benchmark/scripts/run_single_benchmark.sh mysql reads
./benchmark/scripts/run_single_benchmark.sh mysql writes
./benchmark/scripts/run_single_benchmark.sh postgresql reads
./benchmark/scripts/run_single_benchmark.sh postgresql writes
./benchmark/scripts/run_single_benchmark.sh sqlite reads
./benchmark/scripts/run_single_benchmark.sh sqlite writes
