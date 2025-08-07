-- name: GetAuthor :one
SELECT * FROM authors WHERE name = ? LIMIT 1;

-- name: ListAuthors :many
SELECT * 
FROM authors
ORDER BY name
LIMIT ? OFFSET ?;

-- name: CreateAuthor :exec
INSERT INTO authors (id, name, bio) VALUES (?, ?, ?);

-- name: CreateAuthorReturnId :execlastid
INSERT INTO authors (name, bio) VALUES (?, ?);

-- name: GetAuthorById :one
SELECT * FROM authors WHERE id = ? LIMIT 1;

-- name: GetAuthorByNamePattern :many
SELECT * FROM authors
WHERE name LIKE COALESCE(sqlc.narg('name_pattern'), '%');

-- name: DeleteAuthor :exec
DELETE FROM authors
WHERE name = ?;

-- name: DeleteAllAuthors :exec
DELETE FROM authors;

-- name: UpdateAuthors :execrows
UPDATE authors
SET bio = sqlc.arg('bio')
WHERE bio IS NOT NULL;

-- name: GetAuthorsByIds :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids'));

-- name: GetAuthorsByIdsAndNames :many
SELECT * FROM authors WHERE id IN (sqlc.slice('ids')) AND name IN (sqlc.slice('names'));

-- name: CreateBook :execlastid
INSERT INTO books (name, author_id) VALUES (?, ?);

-- name: ListAllAuthorsBooks :many 
SELECT sqlc.embed(authors), sqlc.embed(books) 
FROM authors JOIN books ON authors.id = books.author_id 
ORDER BY authors.name;

-- name: GetDuplicateAuthors :many 
SELECT sqlc.embed(authors1), sqlc.embed(authors2)
FROM authors authors1 JOIN authors authors2 ON authors1.name = authors2.name
WHERE authors1.id < authors2.id;

-- name: GetAuthorsByBookName :many 
SELECT authors.*, sqlc.embed(books)
FROM authors JOIN books ON authors.id = books.author_id
WHERE books.name = ?;

-- name: InsertMysqlTypes :exec
INSERT INTO mysql_types 
(c_bit, c_bool, c_boolean, c_tinyint, c_smallint, c_mediumint, c_int, c_integer, c_bigint, 
 c_decimal, c_dec, c_numeric, c_fixed, c_float, c_double, c_double_precision, 
 c_char, c_nchar, c_national_char, c_varchar, c_tinytext, c_mediumtext, c_text, c_longtext, 
 c_json, c_json_string_override, c_enum, c_set, c_year, c_date, c_datetime, c_timestamp, 
 c_binary, c_varbinary, c_tinyblob, c_blob, c_mediumblob, c_longblob) 
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: InsertMysqlTypesBatch :copyfrom
INSERT INTO mysql_types 
(c_bit, c_bool, c_boolean, c_tinyint, c_smallint, c_mediumint, c_int, c_integer, c_bigint, 
 c_float, c_numeric, c_decimal, c_dec, c_fixed, c_double, c_double_precision, 
 c_char, c_nchar, c_national_char, c_varchar, c_tinytext, c_mediumtext, c_text, c_longtext, 
 c_json, c_json_string_override, c_enum, c_set, c_year, c_date, c_datetime, c_timestamp,
 c_binary, c_varbinary, c_tinyblob, c_blob, c_mediumblob, c_longblob) 
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);

-- name: GetMysqlTypes :one
SELECT * FROM mysql_types LIMIT 1;

-- name: GetMysqlTypesCnt :one
SELECT COUNT(1) AS cnt, c_bool, c_boolean, c_bit, c_tinyint, c_smallint, c_mediumint, c_int, c_integer, c_bigint, 
       c_float, c_numeric, c_decimal, c_dec, c_fixed, c_double, c_double_precision, 
       c_char, c_nchar, c_national_char, c_varchar, c_tinytext, c_mediumtext, c_text, c_longtext, 
       c_json, c_json_string_override, c_enum, c_set, c_year, c_date, c_datetime, c_timestamp, 
       c_binary, c_varbinary, c_tinyblob, c_blob, c_mediumblob, c_longblob
FROM mysql_types
GROUP BY c_bool, c_boolean, c_bit, c_tinyint, c_smallint, c_mediumint, c_int, c_integer, c_bigint, 
         c_float, c_numeric, c_decimal, c_dec, c_fixed, c_double, c_double_precision, 
         c_char, c_nchar, c_national_char, c_varchar, c_tinytext, c_mediumtext, c_text, c_longtext, 
         c_json, c_json_string_override, c_enum, c_set, c_year, c_date, c_datetime, c_timestamp, 
         c_binary, c_varbinary, c_tinyblob, c_blob, c_mediumblob, c_longblob
LIMIT 1;

-- name: GetMysqlFunctions :one
SELECT MAX(c_int) AS max_int, MAX(c_varchar) AS max_varchar, MAX(c_timestamp) AS max_timestamp
FROM mysql_types;

-- name: TruncateMysqlTypes :exec
TRUNCATE TABLE mysql_types;

-- name: CreateExtendedBio :exec
INSERT INTO extended.bios (author_name, name, bio_type) VALUES (?, ?, ?);

-- name: GetFirstExtendedBioByType :one
SELECT * FROM extended.bios WHERE bio_type = ? LIMIT 1;

-- name: TruncateExtendedBios :exec
TRUNCATE TABLE extended.bios;