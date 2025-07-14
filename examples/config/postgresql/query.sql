-- name: GetAuthor :one
SELECT * FROM authors
WHERE name = $1 LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors
ORDER BY name;

-- name: CreateAuthor :one
INSERT INTO authors (id, name, bio) VALUES ($1, $2, $3) RETURNING *;

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES ($1, $2) RETURNING id;

-- name: GetAuthorById :one
SELECT * FROM authors
WHERE id = $1 LIMIT 1;

-- name: GetAuthorByNamePattern :many
SELECT * FROM authors
WHERE name LIKE COALESCE(sqlc.narg('name_pattern'), '%');

-- name: DeleteAuthor :exec
DELETE FROM authors
WHERE name = $1;

-- name: TruncateAuthors :exec
TRUNCATE TABLE authors CASCADE;

-- name: UpdateAuthors :execrows
UPDATE authors
SET bio = $1
WHERE bio IS NOT NULL;

-- name: GetAuthorsByIds :many
SELECT * FROM authors
WHERE id = ANY($1::BIGINT []);

-- name: GetAuthorsByIdsAndNames :many
SELECT *
FROM authors
WHERE id = ANY($1::BIGINT []) AND name = ANY($2::TEXT []);;

-- name: CreateBook :execlastid
INSERT INTO books (name, author_id) VALUES ($1, $2) RETURNING id;

-- name: ListAllAuthorsBooks :many 
SELECT
    sqlc.embed(authors),
    sqlc.embed(books)
FROM authors
INNER JOIN books ON authors.id = books.author_id
ORDER BY authors.name;

-- name: GetDuplicateAuthors :many 
SELECT
    sqlc.embed(authors1),
    sqlc.embed(authors2)
FROM authors AS authors1
INNER JOIN authors AS authors2 ON authors1.name = authors2.name
WHERE authors1.id < authors2.id;

-- name: GetAuthorsByBookName :many 
SELECT
    authors.*,
    sqlc.embed(books)
FROM authors INNER JOIN books ON authors.id = books.author_id
WHERE books.name = $1;

-- name: InsertPostgresTypes :exec
INSERT INTO postgres_types
(
    c_boolean,
    c_bit,
    c_smallint,
    c_integer,
    c_bigint,
    c_real,
    c_numeric,
    c_decimal,
    c_double_precision,
    c_money,
    c_date,
    c_time,
    c_timestamp,
    c_timestamp_with_tz,
    c_char,
    c_varchar,
    c_character_varying,
    c_bpchar,
    c_text,
    c_json,
    c_json_string_override,
    c_jsonb,
    c_bytea, 
    c_boolean_array,
    c_text_array, 
    c_integer_array,
    c_decimal_array,
    c_date_array,
    c_timestamp_array
)
VALUES (
    sqlc.narg('c_boolean'),
    sqlc.narg('c_bit'),
    sqlc.narg('c_smallint'),
    sqlc.narg('c_integer'),
    sqlc.narg('c_bigint'),
    sqlc.narg('c_real'),
    sqlc.narg('c_numeric'),
    sqlc.narg('c_decimal'),
    sqlc.narg('c_double_precision'),
    sqlc.narg('c_money'),
    sqlc.narg('c_date'),
    sqlc.narg('c_time'),
    sqlc.narg('c_timestamp'),
    sqlc.narg('c_timestamp_with_tz'),
    sqlc.narg('c_char'),
    sqlc.narg('c_varchar'),
    sqlc.narg('c_character_varying'),
    sqlc.narg('c_bpchar'),
    sqlc.narg('c_text'),
    sqlc.narg('c_json')::json, 
    sqlc.narg('c_json_string_override')::json, 
    sqlc.narg('c_jsonb')::jsonb,
    sqlc.narg('c_bytea'), 
    sqlc.narg('c_boolean_array'),
    sqlc.narg('c_text_array'), 
    sqlc.narg('c_integer_array'),
    sqlc.narg('c_decimal_array'),
    sqlc.narg('c_date_array'),
    sqlc.narg('c_timestamp_array')
);

-- name: InsertPostgresTypesBatch :copyfrom
INSERT INTO postgres_types
(
    c_boolean,
    c_smallint,
    c_integer,
    c_bigint,
    c_real,
    c_numeric,
    c_decimal,
    c_double_precision,
    c_money,
    c_date,
    c_time,
    c_timestamp,
    c_timestamp_with_tz,
    c_char,
    c_varchar,
    c_character_varying,
    c_bpchar,
    c_text,
    c_bytea
)
VALUES (
    $1, 
    $2, 
    $3, 
    $4, 
    $5, 
    $6, 
    $7, 
    $8, 
    $9, 
    $10, 
    $11, 
    $12, 
    $13, 
    $14, 
    $15, 
    $16, 
    $17, 
    $18,
    $19
);

-- name: GetPostgresTypes :one
SELECT * FROM postgres_types LIMIT 1;

-- name: GetPostgresTypesCnt :one
SELECT
    c_smallint,
    c_boolean,
    c_integer,
    c_bigint,
    c_real,
    c_numeric,
    c_decimal,
    c_double_precision,
    c_money,
    c_date,
    c_time,
    c_timestamp,
    c_timestamp_with_tz,
    c_char,
    c_varchar,
    c_character_varying,
    c_bpchar,
    c_text,
    c_bytea,
    COUNT(*) AS cnt
FROM postgres_types
GROUP BY
    c_smallint,
    c_boolean,
    c_integer,
    c_bigint,
    c_real,
    c_numeric,
    c_decimal,
    c_double_precision,
    c_money,
    c_date,
    c_time,
    c_timestamp,
    c_timestamp_with_tz,
    c_char,
    c_varchar,
    c_character_varying,
    c_bpchar,
    c_text,
    c_bytea
LIMIT 1;

-- name: GetPostgresFunctions :one
SELECT
    MAX(c_integer) AS max_integer,
    MAX(c_varchar) AS max_varchar,
    MAX(c_timestamp) AS max_timestamp
FROM postgres_types;

-- name: InsertPostgresGeoTypes :exec
INSERT INTO postgres_geometric_types (
    c_point, c_line, c_lseg, c_box, c_path, c_polygon, c_circle
)
VALUES ($1, $2, $3, $4, $5, $6, $7);

-- name: InsertPostgresGeoTypesBatch :copyfrom
INSERT INTO postgres_geometric_types (
    c_point, c_line, c_lseg, c_box, c_path, c_polygon, c_circle
)
VALUES ($1, $2, $3, $4, $5, $6, $7);

-- name: GetPostgresGeoTypes :one
SELECT * FROM postgres_geometric_types LIMIT 1;

-- name: TruncatePostgresTypes :exec
TRUNCATE TABLE postgres_types;

-- name: TruncatePostgresGeoTypes :exec
TRUNCATE TABLE postgres_geometric_types;
