CREATE TABLE authors (
    id BIGSERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    bio TEXT
);

CREATE TABLE books (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name TEXT NOT NULL,
    author_id BIGINT NOT NULL,
    description TEXT,
    FOREIGN KEY (author_id) REFERENCES authors (id) ON DELETE CASCADE
);
