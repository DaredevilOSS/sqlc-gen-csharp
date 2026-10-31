-- @description COUNT, COUNT_BIG, SUM, AVG, MIN, MAX and their result types
-- @queries
-- name: BookStats :one
SELECT COUNT(*) AS cnt, COUNT_BIG(*) AS cnt_big, SUM(price) AS total, AVG(price) AS average,
       MIN(published_at) AS first_published, MAX(title) AS last_title, AVG(id) AS avg_id
FROM books;
