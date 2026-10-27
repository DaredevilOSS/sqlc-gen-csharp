-- @description DELETE ... OUTPUT DELETED.*
-- @queries
-- name: DeleteBooksOfAuthor :many
DELETE FROM books OUTPUT DELETED.* WHERE author_id = @author_id;
