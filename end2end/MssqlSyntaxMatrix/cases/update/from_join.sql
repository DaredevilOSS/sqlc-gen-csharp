-- @description UPDATE ... FROM with a JOIN (T-SQL specific)
-- @queries
-- name: MarkAuthorsBooksUnavailable :execrows
UPDATE b SET b.is_available = 0 FROM books b JOIN authors a ON a.id = b.author_id WHERE a.name = @author_name;
