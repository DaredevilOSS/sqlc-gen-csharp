-- @description SEQUENCE used as a column default and with NEXT VALUE FOR in a query
-- @schema
CREATE SEQUENCE ticket_seq AS BIGINT START WITH 1 INCREMENT BY 1;
CREATE TABLE tickets (
    id BIGINT NOT NULL PRIMARY KEY DEFAULT (NEXT VALUE FOR ticket_seq),
    title NVARCHAR(100) NOT NULL
);
-- @queries
-- name: ListTickets :many
SELECT id, title FROM tickets;

-- name: NextTicketId :one
SELECT NEXT VALUE FOR ticket_seq AS next_id;
