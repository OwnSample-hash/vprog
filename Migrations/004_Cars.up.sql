CREATE TABLE Cars (
    Id          INT IDENTITY PRIMARY KEY,
    Name        VARCHAR(255)   NOT NULL,
    SellerId    INT            NOT NULL
        CONSTRAINT Cars_Users_ID_fk
            REFERENCES Users,
    Price       FLOAT          NOT NULL,
    Description TEXT           NOT NULL,
)