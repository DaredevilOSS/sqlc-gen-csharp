-- name: GetAuthor :one
SELECT * FROM authors
WHERE name = $1 LIMIT 1;

-- name: ListAuthors :many
SELECT * 
FROM authors
ORDER BY name
LIMIT sqlc.arg('limit')
OFFSET sqlc.arg('offset');

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
