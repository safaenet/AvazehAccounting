CREATE TABLE [dbo].[InvoiceItems]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [InvoiceId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [BuyPrice] BIGINT NOT NULL , 
    [SellPrice] BIGINT NOT NULL , 
    [CountString] NVARCHAR(50) NOT NULL,
    [CountValue] FLOAT NOT NULL,
    [DateCreated] CHAR(10) NOT NULL, 
    [TimeCreated] CHAR(8) NOT NULL, 
    [DateUpdated] CHAR(10) NULL, 
    [TimeUpdated] CHAR(8) NULL, 
    [Delivered] BIT NOT NULL , 
    [Descriptions] NVARCHAR(50) NULL,
    [ProductUnitId] INT NULL

    CONSTRAINT [FK_Invoices_InvoiceItems] FOREIGN KEY([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
    CONSTRAINT [FK_Invoices_Products] FOREIGN KEY([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    CONSTRAINT [FK_Invoices_ProductUnits] FOREIGN KEY([ProductUnitId]) REFERENCES [ProductUnits] ([Id])
)
GO
