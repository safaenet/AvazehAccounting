CREATE TABLE [dbo].[Products]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ProductName] NVARCHAR(100) NOT NULL, 
    [BuyPrice] [dbo].[CalculationType] NOT NULL ,
    [SellPrice] [dbo].[CalculationType] NOT NULL ,
    [Barcode] NVARCHAR(15) NULL, 
    [CountString] NVARCHAR(50) NULL,
    [DateCreated] [dbo].[DateType] NOT NULL, 
    [TimeCreated] [dbo].[TimeType] NOT NULL, 
    [DateUpdated] [dbo].[DateType] NULL, 
    [TimeUpdated] [dbo].[TimeType] NULL, 
    [Descriptions] NVARCHAR(MAX) NULL, 
    [IsActive] BIT NOT NULL DEFAULT 1 
)