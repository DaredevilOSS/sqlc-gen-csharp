#!/usr/bin/env bash

if [ "$GITHUB_ACTIONS" = "true" ]; then
    echo "Running in Github Actions"
    tests_container_id=$(docker ps -aqf "name=${TESTS_CONTAINER_NAME}")
    tests_exit_code=$(docker wait "${TESTS_CONTAINER_NAME}")
    docker logs --timestamps "${tests_container_id}"
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
fi

exit "${tests_exit_code}"
