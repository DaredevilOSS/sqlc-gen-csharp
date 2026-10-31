-- @description Table-level FOREIGN KEY with ON DELETE CASCADE / SET NULL
-- @schema
CREATE TABLE reviews (
    id INT IDENTITY PRIMARY KEY,
    book_id INT NOT NULL,
    reviewer_id BIGINT NULL,
    CONSTRAINT fk_reviews_book FOREIGN KEY (book_id) REFERENCES books (id) ON DELETE CASCADE,
    CONSTRAINT fk_reviews_reviewer FOREIGN KEY (reviewer_id) REFERENCES authors (id) ON DELETE SET NULL
);
-- @queries
-- name: ListReviews :many
SELECT id, book_id, reviewer_id FROM reviews;
