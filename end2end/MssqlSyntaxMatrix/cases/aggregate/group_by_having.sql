-- @description GROUP BY with HAVING on a parameter
-- @queries
-- name: ProlificAuthors :many
SELECT author_id, COUNT(*) AS book_count FROM books GROUP BY author_id HAVING COUNT(*) >= @min_books;
