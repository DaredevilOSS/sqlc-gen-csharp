#!/usr/bin/env bash

set -ex

if [ "$GITHUB_ACTIONS" = "true" ]; then
    echo "Running in Github Actions"
    dotnet test ./end2end/EndToEndTests/EndToEndTests.csproj
else
    echo "Running in local"
    
    destroy() { docker-compose down --volumes; }
    trap destroy EXIT
    
    docker-compose up --detach --build --force-recreate --remove-orphans --wait
    dotnet test ./end2end/EndToEndTests/EndToEndTests.csproj
fi
