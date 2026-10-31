-- @description LEN, DATALENGTH, SUBSTRING, LEFT, RIGHT, UPPER, LOWER, TRIM, REPLACE, CHARINDEX, PATINDEX, REVERSE, REPLICATE, STUFF
-- @queries
-- name: StringFunctions :many
SELECT LEN(name) AS len, DATALENGTH(name) AS bytes, SUBSTRING(name, 1, 3) AS prefix, LEFT(name, 2) AS left2, RIGHT(name, 2) AS right2,
       UPPER(name) AS upper_name, LOWER(name) AS lower_name, TRIM(name) AS trimmed, LTRIM(RTRIM(name)) AS trimmed2,
       REPLACE(name, N'a', N'b') AS replaced, CHARINDEX(N'a', name) AS pos, PATINDEX(N'%[0-9]%', name) AS digit_pos,
       REVERSE(name) AS reversed, REPLICATE(N'*', 3) AS stars, STUFF(name, 1, 1, N'X') AS stuffed
FROM authors;
