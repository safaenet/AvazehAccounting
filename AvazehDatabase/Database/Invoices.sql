CREATE TABLE [dbo].[Invoices]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1), 
    [CustomerId] INT NOT NULL, 
    [DateCreated] CHAR(10) NOT NULL, 
    [TimeCreated] CHAR(8) NOT NULL, 
    [DateUpdated] CHAR(10) NULL, 
    [TimeUpdated] CHAR(8) NULL, 
    [DiscountType] TINYINT NULL, 
    [DiscountValue] FLOAT NULL, 
    [Descriptions] NTEXT NULL, 
    [LifeStatus] TINYINT NOT NULL 
)