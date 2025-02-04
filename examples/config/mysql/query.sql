-- name: GetAuthor :one
SELECT * FROM authors WHERE name = ? LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :exec
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: GetAuthorById :one
SELECT * FROM authors WHERE id = ? LIMIT 1;

-- name: DeleteAuthor :exec
DELETE FROM authors WHERE name = ?;

-- name: DeleteAllAuthors :exec
DELETE FROM authors;

-- name: TruncateBooks :exec
TRUNCATE TABLE books;

-- name: UpdateAuthors :execrows
UPDATE authors
SET bio = sqlc.arg('bio')
WHERE bio IS NOT NULL;

-- name: SelectAuthorsWithSlice :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids'));

-- name: SelectAuthorsWithTwoSlices :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids')) AND name IN (sqlc.slice('names'));

-- name: CreateBook :exec
INSERT INTO books (name, author_id) VALUES (?, ?);

-- name: ListAllAuthorsBooks :many 
SELECT sqlc.embed(authors), sqlc.embed(books) 
FROM authors JOIN books ON authors.id = books.author_id 
ORDER BY authors.name;

-- name: GetDuplicateAuthors :many 
SELECT sqlc.embed(authors1), sqlc.embed(authors2)
FROM authors authors1 JOIN authors authors2 ON authors1.name = authors2.name
WHERE authors1.id > authors2.id;

-- name: TruncateCopyToTests :exec
TRUNCATE TABLE copy_tests;

-- name: CopyToTests :copyfrom
INSERT INTO copy_tests (c_int, c_varchar, c_date, c_timestamp) VALUES (?, ?, ?, ?);

-- name: CountCopyRows :one
SELECT COUNT(1) AS cnt FROM copy_tests;

/* name: Test :one */
SELECT * FROM node_mysql_types LIMIT 1;