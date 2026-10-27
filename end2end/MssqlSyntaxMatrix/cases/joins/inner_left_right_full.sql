-- @description INNER, LEFT, RIGHT and FULL joins and the nullability they introduce
-- @queries
-- name: InnerJoin :many
SELECT a.name, b.title FROM authors a INNER JOIN books b ON b.author_id = a.id;

-- name: LeftJoin :many
SELECT a.name, b.title, b.price FROM authors a LEFT JOIN books b ON b.author_id = a.id;

-- name: RightJoin :many
SELECT a.name, b.title FROM authors a RIGHT OUTER JOIN books b ON b.author_id = a.id;

-- name: FullJoin :many
SELECT a.name, b.title FROM authors a FULL OUTER JOIN books b ON b.author_id = a.id;
