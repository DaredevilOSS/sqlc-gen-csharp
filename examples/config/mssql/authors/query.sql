-- name: GetAuthor :one
SELECT TOP 1 * FROM authors
WHERE name = @name;

-- name: ListAuthors :many
SELECT *
FROM authors
ORDER BY name
OFFSET sqlc.arg('offset') ROWS FETCH NEXT sqlc.arg('limit') ROWS ONLY;

-- name: CreateAuthor :exec
INSERT INTO authors (id, name, bio) VALUES (@id, @name, @bio);

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) OUTPUT INSERTED.id VALUES (@name, @bio);

-- name: GetAuthorById :one
SELECT TOP 1 * FROM authors -- test rest of line comment
WHERE id = @id;

-- name: GetAuthorByNamePattern :many
SELECT * FROM authors
WHERE name LIKE COALESCE(sqlc.narg('name_pattern'), '%');

-- name: DeleteAuthor :exec
DELETE FROM authors
WHERE name = @name;

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
INSERT INTO books (name, author_id) OUTPUT INSERTED.id VALUES (@name, @author_id);

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
WHERE books.name = @name;
