-- name: GetAuthor :one
SELECT * FROM authors WHERE name = $1 LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :one
INSERT INTO authors (name, bio) VALUES ($1, $2) RETURNING *;

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES ($1, $2) RETURNING id;

-- name: GetAuthorById :one
SELECT * FROM authors WHERE id = $1 LIMIT 1;

-- name: DeleteAuthor :exec
DELETE FROM authors WHERE name = $1;

-- name: TruncateAuthors :exec
TRUNCATE TABLE authors CASCADE;

-- name: UpdateAuthors :execrows
UPDATE authors 
   SET bio = $1
 WHERE bio IS NOT NULL;

-- name: GetAuthorsByIds :many
SELECT * FROM authors WHERE id = ANY($1::BIGINT[]);

-- name: GetAuthorsByIdsAndNames :many
SELECT * FROM authors WHERE id = ANY($1::BIGINT[]) AND name = ANY($2::TEXT[]);;

-- name: CreateBook :exec
INSERT INTO books (name, author_id) VALUES ($1, $2);

-- name: ListAllAuthorsBooks :many 
SELECT sqlc.embed(authors), sqlc.embed(books) FROM authors JOIN books ON authors.id = books.author_id ORDER BY authors.name;

-- name: GetDuplicateAuthors :many 
SELECT sqlc.embed(authors1), sqlc.embed(authors2)
FROM authors authors1 JOIN authors authors2 ON authors1.name = authors2.name
WHERE authors1.id > authors2.id;

-- name: GetAuthorsByBookName :many 
SELECT authors.*, sqlc.embed(books)
FROM authors JOIN books ON authors.id = books.author_id
WHERE books.name = $1;

-- name: TruncateCopyToTests :exec
TRUNCATE TABLE copy_tests;

-- name: CopyToTests :copyfrom
INSERT INTO copy_tests (c_int, c_varchar, c_date, c_timestamp)
VALUES ($1, $2, $3, $4);

-- name: GetCopyStats :one
SELECT
    COUNT(1) AS cnt,
    MAX(c_int)::int AS c_int,
    MAX(c_varchar)::varchar AS c_varchar,
    MAX(c_date)::date AS c_date,
    MAX(c_timestamp)::timestamp AS c_timestamp
FROM copy_tests;

-- name: InsertPostgresTypes :execlastid
INSERT INTO postgres_types (c_smallint, c_boolean, c_integer, c_bigint, c_serial, c_decimal, c_numeric, c_real, c_date, c_timestamp, c_char, c_varchar, c_character_varying, c_text, c_text_array, c_integer_array)
VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15, $16) RETURNING id;

-- name: GetPostgresTypes :one
SELECT * FROM postgres_types WHERE id = $1 LIMIT 1;

-- name: TruncatePostgresTypes :exec
TRUNCATE TABLE postgres_types;