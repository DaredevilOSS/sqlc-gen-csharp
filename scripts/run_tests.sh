#!/usr/bin/env bash

if [ "$GITHUB_ACTIONS" = "true" ]; then
    echo "Running in Github Actions"
    echo "the value of TESTS_CONTAINER_NAME is $TESTS_CONTAINER_NAME"
    export TESTS_CONTAINER_NAME="$1"
    
    tests_container_id=$(docker ps -aqf "name=${TESTS_CONTAINER_NAME}")
    tests_exit_code=$(docker wait "${TESTS_CONTAINER_NAME}")
else
    echo "Running in local"
    set -ex

    destroy() { docker-compose down --volumes; }
    trap destroy EXIT

    source .env
    docker-compose up --detach --build --force-recreate --remove-orphans
    tests_container_id=$(docker ps -aqf "name=${TESTS_CONTAINER_NAME}")
    tests_exit_code=$(docker wait "${TESTS_CONTAINER_NAME}")

    # by container_id since the container is dead
    docker logs --timestamps "${tests_container_id}" > tests.log && cat tests.log
    exit "${tests_exit_code}"
fi
