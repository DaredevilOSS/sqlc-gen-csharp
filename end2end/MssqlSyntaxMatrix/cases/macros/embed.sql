-- @description sqlc.embed of a table, of aliases (self join) and next to a t.* star
-- @queries
-- name: AuthorsWithBooks :many
SELECT sqlc.embed(authors), sqlc.embed(books) FROM authors JOIN books ON books.author_id = authors.id;

-- name: DuplicateAuthors :many
SELECT sqlc.embed(a1), sqlc.embed(a2) FROM authors a1 JOIN authors a2 ON a1.name = a2.name AND a1.id < a2.id;

-- name: BooksWithAuthor :many
SELECT books.*, sqlc.embed(authors) FROM books JOIN authors ON authors.id = books.author_id WHERE books.title = @title;
