-- @description CTEs in front of UPDATE and DELETE
-- @queries
-- name: DiscountTopBooks :execrows
WITH top_books AS (SELECT TOP (@n) * FROM books ORDER BY price DESC)
UPDATE top_books SET price = price * 0.9;

-- name: DeleteDuplicates :execrows
WITH dups AS (SELECT id, ROW_NUMBER() OVER (PARTITION BY title ORDER BY id) AS rn FROM books)
DELETE FROM dups WHERE rn > 1;
