-- @description DECLARE / SET local variables before the final SELECT
-- @queries
-- name: BooksSinceDaysAgo :many
DECLARE @since DATETIME2 = DATEADD(DAY, -@days, SYSUTCDATETIME());
SELECT id, title FROM books WHERE published_at >= @since;
