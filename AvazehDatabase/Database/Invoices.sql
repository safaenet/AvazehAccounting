CREATE TABLE [dbo].[Invoices]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [CustomerId] INT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL, 
    [DiscountType] TINYINT NULL DEFAULT 0, 
    [DiscountValue] FLOAT NULL DEFAULT 0, 
    [Descriptions] NTEXT NULL, 
    [LifeStatus] TINYINT NOT NULL,
    [PrevInvoiceId] INT NULL
)