-- @description TRY ... CATCH with THROW
-- @queries
-- name: SafeDelete :exec
BEGIN TRY
    DELETE FROM authors WHERE id = @id;
END TRY
BEGIN CATCH
    THROW;
END CATCH;
