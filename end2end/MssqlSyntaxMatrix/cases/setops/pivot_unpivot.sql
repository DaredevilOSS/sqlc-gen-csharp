-- @description PIVOT and UNPIVOT
-- @schema
CREATE TABLE quarterly (region NVARCHAR(10) NOT NULL, q1 INT NOT NULL, q2 INT NOT NULL);
-- @queries
-- name: AvailabilityPivot :many
SELECT author_id, [0] AS unavailable, [1] AS available
FROM (SELECT author_id, CAST(is_available AS INT) AS is_available, id FROM books) src
PIVOT (COUNT(id) FOR is_available IN ([0], [1])) AS p;

-- name: QuarterUnpivot :many
SELECT region, quarter, amount
FROM (SELECT region, q1, q2 FROM quarterly) src
UNPIVOT (amount FOR quarter IN (q1, q2)) AS u;
