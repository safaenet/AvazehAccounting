CREATE TABLE [dbo].[Products]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1), 
    [ProductName] NVARCHAR(100) NOT NULL, 
    [BuyPrice] BIGINT NOT NULL ,
    [SellPrice] BIGINT NOT NULL ,
    [Barcode] VARCHAR(15) NULL, 
    [CountString] NCHAR(50) NULL,
    [DateCreated] CHAR(10) NOT NULL, 
    [TimeCreated] CHAR(8) NOT NULL, 
    [DateUpdated] CHAR(10) NULL, 
    [TimeUpdated] CHAR(8) NULL, 
    [Descriptions] NTEXT NULL
)