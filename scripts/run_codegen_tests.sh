#!/usr/bin/env bash

file_per_query=$1
mysql_file_count=$(find MySqlConnectorExample/ -maxdepth 1 -name "*.cs" 2>/dev/null | wc -l)

if [ "${file_per_query}" = "true" ]; then
    if [ "${mysql_file_count}" -gt 2 ]; then
        echo "Assertion passed: ${mysql_file_count} > 2 .cs files are present in the directory MySqlConnectorExample."
    else
        echo "Assertion failed: ${mysql_file_count} <= 2 .cs files in the directory MySqlConnectorExample."
        exit 1
    fi
elif [ "${file_per_query}" = "false" ]; then
    if [ "${mysql_file_count}" -eq 2 ]; then
        echo "Assertion passed: Exactly 2 .cs files are present in the directory MySqlConnectorExample."
    else
        echo "Assertion failed: ${mysql_file_count} != 2 .cs files in the directory MySqlConnectorExample."
        exit 1
    fi
else
    echo "Invalid input. Please provide 'true' or 'false'."
    exit 1
fi