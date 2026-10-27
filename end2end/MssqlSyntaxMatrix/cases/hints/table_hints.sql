-- @description Table hints: NOLOCK, UPDLOCK, ROWLOCK, READPAST, FORCESEEK
-- @queries
-- name: ReadUncommitted :many
SELECT id FROM authors WITH (NOLOCK);

-- name: LockForUpdate :one
SELECT id, name FROM authors WITH (UPDLOCK, ROWLOCK) WHERE id = @id;

-- name: DequeueLike :one
SELECT TOP 1 id FROM books WITH (READPAST, UPDLOCK) WHERE is_available = 1;
