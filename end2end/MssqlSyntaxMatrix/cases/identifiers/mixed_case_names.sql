-- @description Mixed-case table and column names map to PascalCase
-- @schema
CREATE TABLE CustomerAccounts (CustomerID INT NOT NULL PRIMARY KEY, firstName NVARCHAR(50) NOT NULL, LAST_NAME NVARCHAR(50) NOT NULL);
-- @queries
-- name: ListMixed :many
SELECT CustomerID, firstName, LAST_NAME FROM CustomerAccounts WHERE CustomerID = @CustomerID;
