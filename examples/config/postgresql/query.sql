-- name: GetAuthor :one
SELECT * FROM authors WHERE name = $1 LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :one
INSERT INTO authors (id, name, bio) VALUES ($1, $2, $3) RETURNING *;

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES ($1, $2) RETURNING id;

-- name: GetAuthorById :one
SELECT * FROM authors WHERE id = $1 LIMIT 1;

-- name: GetAuthorByNamePattern :many
SELECT * FROM authors WHERE name LIKE COALESCE(sqlc.narg('name_pattern'), '%');

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

-- name: CreateBook :execlastid
INSERT INTO books (name, author_id) VALUES ($1, $2) RETURNING id;

-- name: ListAllAuthorsBooks :many 
SELECT sqlc.embed(authors), sqlc.embed(books) FROM authors JOIN books ON authors.id = books.author_id ORDER BY authors.name;

-- name: GetDuplicateAuthors :many 
SELECT sqlc.embed(authors1), sqlc.embed(authors2)
FROM authors authors1 JOIN authors authors2 ON authors1.name = authors2.name
WHERE authors1.id < authors2.id;

-- name: GetAuthorsByBookName :many 
SELECT authors.*, sqlc.embed(books)
FROM authors JOIN books ON authors.id = books.author_id
WHERE books.name = $1;

-- name: InsertPostgresTypes :exec
INSERT INTO postgres_types (c_smallint, c_boolean, c_integer, c_bigint, c_decimal, c_numeric, c_real, c_date, c_timestamp, c_char, c_varchar, c_character_varying, c_text, c_text_array, c_integer_array)
VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15);

-- name: InsertPostgresTypesBatch :copyfrom
INSERT INTO postgres_types (c_smallint, c_boolean, c_integer, c_bigint, c_decimal, c_numeric, c_real, c_date, c_timestamp, c_char, c_varchar, c_character_varying, c_text)
VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13);

-- name: GetPostgresTypes :one
SELECT * FROM postgres_types LIMIT 1;

-- name: GetPostgresTypesAgg :one
SELECT COUNT(1) AS cnt , c_smallint, c_boolean, c_integer, c_bigint, c_decimal, c_numeric, c_real, c_date, c_timestamp, c_char, c_varchar, c_character_varying, c_text
FROM postgres_types
GROUP BY c_smallint, c_boolean, c_integer, c_bigint, c_decimal, c_numeric, c_real, c_date, c_timestamp, c_char, c_varchar, c_character_varying, c_text
LIMIT 1;

-- name: TruncatePostgresTypes :exec
TRUNCATE TABLE postgres_types;