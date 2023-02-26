CREATE TABLE [dbo].[Products]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ProductName] NVARCHAR(100) NOT NULL, 
    [BuyPrice] BIGINT NOT NULL ,
    [SellPrice] BIGINT NOT NULL ,
    [Barcode] NVARCHAR(15) NULL, 
    [CountString] NVARCHAR(50) NULL,
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] CHAR(10) NULL, 
    [Descriptions] NTEXT NULL, 
    [IsActive] BIT NOT NULL DEFAULT 1 
)