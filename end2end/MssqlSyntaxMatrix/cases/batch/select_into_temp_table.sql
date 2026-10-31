-- @description SELECT ... INTO #temp then reading it, in one query
-- @queries
-- name: ExpensiveBooksViaTemp :many
SELECT id, title INTO #expensive FROM books WHERE price > @min_price;
SELECT id, title FROM #expensive;
