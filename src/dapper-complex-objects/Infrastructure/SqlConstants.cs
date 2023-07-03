namespace DapperComplexObjects.Infrastructure;
public static class SqlConstants
{
    public const string CreateAccounts = @"
                    CREATE TABLE Accounts(
                        Id UNIQUEIDENTIFIER PRIMARY KEY,
                        Description NVARCHAR(255) NOT NULL,
                        TotalValue DECIMAL(18,2) NOT NULL
                    );";


    public const string CreateInstallments = @"
                    CREATE TABLE Installments(
                        Id UNIQUEIDENTIFIER PRIMARY KEY,
                        DueDate DATETIME NOT NULL,
                        Value DECIMAL(18,2) NOT NULL,
                        AccountId UNIQUEIDENTIFIER NOT NULL,
                        FOREIGN KEY(AccountId) REFERENCES Accounts(Id)
                    );";


    public const string DropAccountAndInstallments = @"
                    DROP TABLE Installments; 
                    DROP TABLE Accounts;";

    public const string ClassicQuery = @"
                    SELECT acc.*, i.*  FROM Accounts acc
                    inner join Installments i ON i.AccountId = acc.Id";

    public const string MultipleQuery = @"
                    SELECT * FROM Accounts;
                    SELECT * FROM Installments;";

    public const string CreateAccountsJson = @"
                    CREATE TABLE AccountsJson(
                        Id UNIQUEIDENTIFIER PRIMARY KEY,
                        Description NVARCHAR(255) NOT NULL,
                        TotalValue DECIMAL(18,2) NOT NULL,
                        Installments NVARCHAR(MAX) NULL
                    );";

    public const string DropAccountsJson = "DROP TABLE AccountsJson;";

    public const string JsonQuery = "SELECT * FROM AccountsJson";

    public const string CreateAccountsSpan = @"
                    CREATE TABLE AccountsSpan(
                        Id UNIQUEIDENTIFIER PRIMARY KEY,
                        Description NVARCHAR(255) NOT NULL,
                        TotalValue DECIMAL(18,2) NOT NULL,
                        Installments VARBINARY(MAX) NULL
                    );";

    public const string DropAccountsSpan = "DROP TABLE AccountsSpan;";

    public const string SpanQuery = "SELECT * FROM AccountsSpan";

}
