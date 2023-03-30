CREATE TABLE [dbo].[Invoices]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [CustomerId] INT NOT NULL, 
    [DateCreated] CHAR(10) NOT NULL, 
    [TimeCreated] CHAR(8) NOT NULL, 
    [DateUpdated] CHAR(10) NULL, 
    [TimeUpdated] CHAR(8) NULL, 
    [DiscountType] TINYINT NULL DEFAULT 0, 
    [DiscountValue] DECIMAL NULL DEFAULT 0, 
    [About] NVARCHAR(50) NULL,
    [Descriptions] NTEXT NULL, 
    [LifeStatus] TINYINT NOT NULL,
    [PrevInvoiceId] INT NULL
 --   [BuySum] AS (dbo.CalculateInvoiceBuySum([Id])) PERSISTED,
 --   [SellSum] AS (dbo.CalculateInvoiceSellSum([Id])) PERSISTED,
 --   [TotalSum] AS (dbo.CalculateDiscountedInvoiceSum([DiscountType], [DiscountValue], dbo.CalculateInvoiceSellSum([Id]))) PERSISTED,
	--[NetProfit]  AS (dbo.CalculateDiscountedInvoiceSum([DiscountType], [DiscountValue], dbo.CalculateInvoiceSellSum([Id])) - dbo.CalculateInvoiceBuySum([Id])) PERSISTED
)