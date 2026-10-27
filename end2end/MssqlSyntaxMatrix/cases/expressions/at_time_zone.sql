-- @description AT TIME ZONE and SWITCHOFFSET
-- @queries
-- name: InTimeZone :many
SELECT published_at AT TIME ZONE 'UTC' AT TIME ZONE @time_zone AS local_time,
       SWITCHOFFSET(SYSDATETIMEOFFSET(), '+02:00') AS switched
FROM books;
