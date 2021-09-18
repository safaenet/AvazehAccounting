CREATE TABLE [dbo].[ProductPrices]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[ProductId] INT NOT NULL,
	[UnitType] INT NOT NULL,
	[BuyPrice] BIGINT NULL, 
    [SellPrice] BIGINT NULL, 
    [Count_] INT NULL, 
    [DateEntered] CHAR(10) NULL, 
    [TimeEntered] CHAR(8) NULL
	
)
