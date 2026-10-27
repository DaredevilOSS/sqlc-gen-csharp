-- @description UNION, UNION ALL, EXCEPT and INTERSECT
-- @queries
-- name: UnionNames :many
SELECT name FROM authors UNION SELECT title FROM books;

-- name: UnionAllIds :many
SELECT id FROM authors UNION ALL SELECT author_id FROM books;

-- name: AuthorsWithoutBooks :many
SELECT id FROM authors EXCEPT SELECT author_id FROM books;

-- name: AuthorsWithBooks :many
SELECT id FROM authors INTERSECT SELECT author_id FROM books;
