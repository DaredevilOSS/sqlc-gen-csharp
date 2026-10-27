-- @description Columns in a table-level (composite) PRIMARY KEY are implicitly NOT NULL
-- @schema
CREATE TABLE memberships (
    group_id INT,
    user_id INT,
    role NVARCHAR(20),
    CONSTRAINT pk_memberships PRIMARY KEY (group_id, user_id)
);
-- @queries
-- name: ListMemberships :many
SELECT group_id, user_id, role FROM memberships;
