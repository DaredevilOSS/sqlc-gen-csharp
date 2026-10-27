-- @description Recursive CTE with UNION ALL
-- @queries
-- name: Numbers :many
WITH numbers (n) AS (
    SELECT 1
    UNION ALL
    SELECT n + 1 FROM numbers WHERE n < @upto
)
SELECT n FROM numbers OPTION (MAXRECURSION 1000);
