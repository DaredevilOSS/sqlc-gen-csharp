-- @description Schema split into several batches with GO
-- @schema
CREATE TABLE countries (id INT PRIMARY KEY, name NVARCHAR(50) NOT NULL);
GO
CREATE TABLE regions (id INT PRIMARY KEY, country_id INT NOT NULL REFERENCES countries (id), name NVARCHAR(50) NOT NULL);
GO
-- @queries
-- name: ListRegions :many
SELECT r.id, r.name, c.name AS country FROM regions r JOIN countries c ON c.id = r.country_id;
