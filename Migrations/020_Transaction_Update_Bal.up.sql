CREATE OR ALTER TRIGGER Transaction_Update_Bal
    ON dbo.Transactions
    AFTER INSERT AS
BEGIN
    UPDATE u SET u.Balance = u.Balance - i.Amount FROM dbo.Users u JOIN inserted i ON u.ID = i.AccountId;
    UPDATE u SET u.Balance = u.Balance + i.Amount FROM dbo.Users u JOIN inserted i ON u.ID = i.SellerId;
END