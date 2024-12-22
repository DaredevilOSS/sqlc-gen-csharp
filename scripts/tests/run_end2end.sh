#!/usr/bin/env bash

set -ex

if [ "$GITHUB_ACTIONS" = "true" ]; then
    echo "Running in Github Actions"
    core_tests_id=$(docker ps -aqf "name=${TESTS_DOTNET_CORE_CONTAINER}")
    core_exit_code=$(docker wait "${TESTS_DOTNET_CORE_CONTAINER}")
    docker logs --timestamps "${core_tests_id}"
    framework_tests_id=$(docker ps -aqf "name=${TESTS_DOTNET_FRAMEWORK_CONTAINER}")
    framework_exit_code=$(docker wait "${TESTS_DOTNET_FRAMEWORK_CONTAINER}")
    docker logs --timestamps "${framework_tests_id}"
else
    echo "Running in local"
    set -ex

    destroy() { docker-compose down --volumes; }
    trap destroy EXIT

    source .env
    docker-compose up --detach --build --force-recreate --remove-orphans
    
    core_tests_id=$(docker ps -aqf "name=${TESTS_DOTNET_CORE_CONTAINER}")
    core_exit_code=$(docker wait "${TESTS_DOTNET_CORE_CONTAINER}")
    if [ "${core_exit_code}" -ne 0 ]; then
      docker logs --timestamps "${core_tests_id}"
      exit "${core_exit_code}"
    fi
    
    framework_tests_id=$(docker ps -aqf "name=${TESTS_DOTNET_FRAMEWORK_CONTAINER}")
    framework_exit_code=$(docker wait "${TESTS_DOTNET_FRAMEWORK_CONTAINER}")

    if [ "${framework_exit_code}" -ne 0 ]; then
      docker logs --timestamps "${framework_tests_id}" > tests.log
      exit "${framework_exit_code}"
    fi
fi
