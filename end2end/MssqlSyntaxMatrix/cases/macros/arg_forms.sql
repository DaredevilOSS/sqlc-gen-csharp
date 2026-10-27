-- @description sqlc.arg with a quoted name, a bare identifier and a double-quoted name
-- @queries
-- name: ArgForms :many
SELECT id FROM authors WHERE name = sqlc.arg('author_name') AND id > sqlc.arg(min_id) AND bio <> sqlc.arg("bio_text");
