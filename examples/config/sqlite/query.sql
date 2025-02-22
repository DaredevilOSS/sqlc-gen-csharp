-- name: GetAuthor :one
SELECT * FROM authors WHERE name = ? LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :exec
INSERT INTO authors (id, name, bio) VALUES (?, ?, ?);

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES (?, ?) RETURNING id;

-- name: GetAuthorById :one
SELECT * FROM authors WHERE id = ? LIMIT 1;

-- name: GetAuthorByNamePattern :many
SELECT * FROM authors WHERE name LIKE COALESCE(sqlc.narg('name_pattern'), '%');

-- name: UpdateAuthors :execrows
UPDATE authors 
SET bio = ?
WHERE bio IS NOT NULL;

-- name: GetAuthorsByIds :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids'));

-- name: GetAuthorsByIdsAndNames :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids')) AND name IN (sqlc.slice('names'));

-- name: DeleteAuthor :exec
DELETE FROM authors WHERE name = ?;

-- name: CreateBook :execlastid
INSERT INTO books (name, author_id) VALUES (?, ?) RETURNING id;

-- name: ListAllAuthorsBooks :many 
SELECT sqlc.embed(authors), sqlc.embed(books) 
FROM authors JOIN books ON authors.id = books.author_id 
ORDER BY authors.name;

-- name: GetDuplicateAuthors :many 
SELECT sqlc.embed(authors1), sqlc.embed(authors2)
FROM authors authors1 JOIN authors authors2 ON authors1.name = authors2.name
WHERE authors1.id < authors2.id;

-- name: GetAuthorsByBookName :many 
SELECT authors.*, sqlc.embed(books)
FROM authors JOIN books ON authors.id = books.author_id
WHERE books.name = ?;

-- name: DeleteAllAuthors :exec
DELETE FROM authors;

-- name: InsertSqliteTypes :exec
INSERT INTO types_sqlite (c_integer, c_real, c_text, c_blob) VALUES (?, ?, ?, ?);

-- name: InsertSqliteTypesBatch :copyfrom
INSERT INTO types_sqlite (c_integer, c_real, c_text) VALUES (?, ?, ?);

-- name: GetSqliteTypes :one
SELECT * FROM types_sqlite LIMIT 1;

-- name: GetSqliteTypesAgg :one
SELECT COUNT(1) AS cnt , c_integer, c_real, c_text, c_blob
FROM types_sqlite
GROUP BY c_integer, c_real, c_text, c_blob
LIMIT 1;

-- name: DeleteAllSqliteTypes :exec
DELETE FROM types_sqlite;