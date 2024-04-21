#!/usr/bin/env bash

set -ex

destroy() { docker-compose down --volumes; }
trap destroy EXIT

docker-compose up --detach --build --force-recreate --remove-orphans
tests_container_id=$(docker ps -aqf "name=plugin-tests")
tests_exit_code=$(docker wait plugin-tests)

# by container_id since the container is dead
docker logs --timestamps "${tests_container_id}" > tests.log && cat tests.log
exit "${tests_exit_code}"
