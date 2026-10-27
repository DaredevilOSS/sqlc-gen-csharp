-- @description DELETE ... FROM with a JOIN
-- @queries
-- name: DeleteBooksByAuthorName :execrows
DELETE b FROM books b JOIN authors a ON a.id = b.author_id WHERE a.name = @author_name;
