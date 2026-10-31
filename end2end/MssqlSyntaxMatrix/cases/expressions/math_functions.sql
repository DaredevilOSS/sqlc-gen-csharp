-- @description ABS, ROUND, CEILING, FLOOR, POWER, SQRT, SIGN, PI, RAND
-- @queries
-- name: MathFunctions :many
SELECT ABS(price) AS abs_price, ROUND(price, 1) AS rounded, CEILING(price) AS ceil, FLOOR(price) AS floor_price,
       POWER(id, 2) AS squared, SQRT(id) AS root, SIGN(price) AS sign, PI() AS pi, RAND() AS random
FROM books;
