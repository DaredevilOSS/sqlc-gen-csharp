-- @description A table-valued parameter (user-defined table type) used in a join
-- @schema
CREATE TYPE dbo.id_list AS TABLE (id BIGINT NOT NULL PRIMARY KEY);
-- @queries
-- name: AuthorsByIdList :many
SELECT a.id, a.name FROM authors a JOIN @ids i ON i.id = a.id;
