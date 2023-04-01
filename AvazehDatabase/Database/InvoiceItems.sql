CREATE TABLE [dbo].[InvoiceItems]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [InvoiceId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [BuyPrice] [dbo].[CalculationType] NOT NULL , 
    [SellPrice] [dbo].[CalculationType] NOT NULL , 
    [CountString] NVARCHAR(50) NOT NULL,
    [CountValue] [dbo].[CountType] NOT NULL,
    [DateCreated] [dbo].[DateType] NOT NULL, 
    [TimeCreated] [dbo].[TimeType] NOT NULL, 
    [DateUpdated] [dbo].[DateType] NULL, 
    [TimeUpdated] [dbo].[TimeType] NULL, 
    [Delivered] BIT NOT NULL , 
    [Descriptions] NVARCHAR(50) NULL,
    [ProductUnitId] INT NULL

    CONSTRAINT [FK_Invoices_InvoiceItems] FOREIGN KEY([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
    CONSTRAINT [FK_Invoices_Products] FOREIGN KEY([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    CONSTRAINT [FK_Invoices_ProductUnits] FOREIGN KEY([ProductUnitId]) REFERENCES [ProductUnits] ([Id])
)
GO
