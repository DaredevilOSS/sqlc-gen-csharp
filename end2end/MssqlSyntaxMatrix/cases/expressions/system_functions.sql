-- @description NEWID, @@ROWCOUNT, SCOPE_IDENTITY, IDENT_CURRENT, DB_NAME, SUSER_SNAME, HASHBYTES, CHECKSUM
-- @queries
-- name: SystemValues :one
SELECT NEWID() AS id, @@ROWCOUNT AS row_count, SCOPE_IDENTITY() AS scope_id, IDENT_CURRENT('authors') AS current_id,
       DB_NAME() AS db, SUSER_SNAME() AS login, HASHBYTES('SHA2_256', @payload) AS hash, CHECKSUM(@payload) AS checksum;
