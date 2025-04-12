-- name: TestOne :one
SELECT * FROM dummy_table LIMIT 1;

-- name: TestInsert :exec
INSERT INTO dummy_table (dummy_column) VALUES (?);