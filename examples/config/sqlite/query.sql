-- name: GetAuthor :one
SELECT * FROM authors WHERE name = ? LIMIT 1;

-- name: ListAuthors :many
SELECT * FROM authors ORDER BY name;

-- name: CreateAuthor :exec
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES (?, ?) RETURNING id;

-- name: GetAuthorById :one
SELECT * FROM authors WHERE id = ? LIMIT 1;

-- name: UpdateAuthors :execrows
UPDATE authors 
SET bio = ?
WHERE bio IS NOT NULL;

-- name: SelectAuthorsWithSlice :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids'));

-- name: SelectAuthorsWithTwoSlices :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids')) AND name IN (sqlc.slice('names'));

-- name: DeleteAuthor :exec
DELETE FROM authors WHERE name = ?;

-- name: CreateBook :exec
INSERT INTO books (name, author_id) VALUES (?, ?);

-- name: ListAllAuthorsBooks :many 
SELECT sqlc.embed(authors), sqlc.embed(books) FROM authors JOIN books ON authors.id = books.author_id ORDER BY authors.name;

-- name: GetDuplicateAuthors :many 
SELECT sqlc.embed(authors1), sqlc.embed(authors2)
FROM authors authors1 JOIN authors authors2 ON authors1.name = authors2.name
WHERE authors1.id > authors2.id;

-- name: DeleteAllAuthors :exec
DELETE FROM authors;

-- name: DeleteAllBooks :exec
DELETE FROM books;