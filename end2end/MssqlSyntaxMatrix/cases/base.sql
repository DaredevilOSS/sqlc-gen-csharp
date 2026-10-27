-- Shared by every case; a case adds its own objects in its -- @schema section.
CREATE TABLE authors (
    id BIGINT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    bio NVARCHAR(MAX) NULL,
    birth_date DATE NULL
);

CREATE TABLE books (
    id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    author_id BIGINT NOT NULL REFERENCES authors (id),
    title NVARCHAR(200) NOT NULL,
    price DECIMAL(10, 2) NULL,
    published_at DATETIME2 NULL,
    is_available BIT NOT NULL DEFAULT 1
);
