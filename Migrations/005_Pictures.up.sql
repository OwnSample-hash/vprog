CREATE TABLE Pictures (
    Id        INT IDENTITY PRIMARY KEY,
    CarId     INT            NULL
        CONSTRAINT Pictures_Cars_ID_fk
            REFERENCES Cars,
    Url       VARCHAR(255)   NOT NULL,
    IsCached  BIT            NOT NULL DEFAULT 0,
    CachePath VARCHAR(255)   NULL
);