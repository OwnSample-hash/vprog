CREATE TABLE Transactions (
	Id INT PRIMARY KEY IDENTITY(0,1),
	Amount DECIMAL(18, 2) NOT NULL,
	Date DATE NOT NULL,
	AccountId INT NOT NULL
		CONSTRAINT Transactions_Users_ID_fk
			REFERENCES Users,
	CarID INT NOT NULL
		CONSTRAINT Transactions_Cars_ID_fk
			REFERENCES Cars,
	SellerId INT NOT NULL
		CONSTRAINT Transactions_Sellers_ID_fk
			REFERENCES Users
);