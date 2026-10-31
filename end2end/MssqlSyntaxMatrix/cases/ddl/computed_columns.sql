-- @description Computed columns, plain and PERSISTED
-- @schema
CREATE TABLE people (
    id INT IDENTITY PRIMARY KEY,
    first_name NVARCHAR(50) NOT NULL,
    last_name NVARCHAR(50) NOT NULL,
    full_name AS (first_name + N' ' + last_name),
    name_length AS (LEN(first_name) + LEN(last_name)) PERSISTED
);
-- @queries
-- name: ListPeople :many
SELECT * FROM people;
