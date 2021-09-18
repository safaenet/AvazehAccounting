CREATE TABLE [dbo].[CustomerizedPrices]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [CustomerId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [SellPrice] BIGINT NOT NULL, 
    [DateAdded] CHAR(10) NOT NULL, 
    CONSTRAINT [FK_CustomerizedPrices_ToCustomers] FOREIGN KEY ([CustomerId]) REFERENCES [Customers]([Id]), 
    CONSTRAINT [FK_CustomerizedPrices_ToProduts] FOREIGN KEY (ProductId) REFERENCES [Products]([Id]),
)
