CREATE TABLE [dbo].[Invoices]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [CustomerId] INT NOT NULL, 
    [DateCreated] [dbo].[DateType] NOT NULL, 
    [TimeCreated] [dbo].[TimeType] NOT NULL, 
    [DateUpdated] [dbo].[DateType] NULL, 
    [TimeUpdated] [dbo].[TimeType] NULL, 
    [DiscountType] TINYINT NULL DEFAULT 0, 
    [DiscountValue] DECIMAL(18, 1) NULL DEFAULT 0, 
    [About] NVARCHAR(50) NULL,
    [Descriptions] NVARCHAR(MAX) NULL, 
    [LifeStatus] TINYINT NOT NULL,
    [PrevInvoiceId] INT NULL
 --   [BuySum] AS (dbo.CalculateInvoiceBuySum([Id])) PERSISTED,
 --   [SellSum] AS (dbo.CalculateInvoiceSellSum([Id])) PERSISTED,
 --   [TotalSum] AS (dbo.CalculateDiscountedInvoiceSum([DiscountType], [DiscountValue], dbo.CalculateInvoiceSellSum([Id]))) PERSISTED,
	--[NetProfit]  AS (dbo.CalculateDiscountedInvoiceSum([DiscountType], [DiscountValue], dbo.CalculateInvoiceSellSum([Id])) - dbo.CalculateInvoiceBuySum([Id])) PERSISTED
)