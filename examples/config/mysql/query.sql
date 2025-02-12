-- name: GetAuthor :one
SELECT * FROM authors WHERE name = ? LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :exec
INSERT INTO authors (id, name, bio) VALUES (?, ?, ?);

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: GetAuthorById :one
SELECT * FROM authors WHERE id = ? LIMIT 1;

-- name: DeleteAuthor :exec
DELETE FROM authors WHERE name = ?;

-- name: DeleteAllAuthors :exec
DELETE FROM authors;

-- name: UpdateAuthors :execrows
UPDATE authors
SET bio = sqlc.arg('bio')
WHERE bio IS NOT NULL;

-- name: GetAuthorsByIds :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids'));

-- name: GetAuthorsByIdsAndNames :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids')) AND name IN (sqlc.slice('names'));

-- name: CreateBook :execlastid
INSERT INTO books (name, author_id) VALUES (?, ?);

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

-- name: InsertMysqlTypes :exec
INSERT INTO mysql_types (c_bit, c_tinyint, c_bool, c_boolean, c_int, c_varchar, c_date, c_timestamp) VALUES (?, ?, ?, ?, ?, ?, ?, ?);

-- name: InsertMysqlTypesBatch :copyfrom
INSERT INTO mysql_types (c_bit, c_tinyint, c_bool, c_boolean, c_int, c_varchar, c_date, c_timestamp) VALUES (?, ?, ?, ?, ?, ?, ?, ?);

-- name: GetMysqlTypes :one
SELECT * FROM mysql_types LIMIT 1;

-- name: GetMysqlTypesAgg :one
SELECT COUNT(1) AS cnt , c_bit, c_tinyint, c_bool, c_boolean, c_int, c_varchar, c_date, c_timestamp
FROM mysql_types
GROUP BY c_bit, c_tinyint, c_bool, c_boolean, c_int, c_varchar, c_date, c_timestamp
LIMIT 1;

-- name: TruncateMysqlTypes :exec
TRUNCATE TABLE mysql_types;