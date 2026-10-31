-- @description User-defined alias type (CREATE TYPE ... FROM)
-- @schema
CREATE TYPE dbo.email_address FROM NVARCHAR(320) NOT NULL;
GO
CREATE TABLE contacts (
    id INT IDENTITY PRIMARY KEY,
    email dbo.email_address
);
-- @queries
-- name: ListContacts :many
SELECT id, email FROM contacts;

-- name: GetContactByEmail :one
SELECT id FROM contacts WHERE email = @email;
