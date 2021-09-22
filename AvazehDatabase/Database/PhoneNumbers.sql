CREATE TABLE [dbo].[PhoneNumbers]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[CustomerId] INT NOT NULL,
    [PhoneNumber] NVARCHAR(16) NOT NULL,

	--CHECK ([PhoneNumber] LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR [PhoneNumber] LIKE '+[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'), 
	CONSTRAINT [FK_PhoneNumbers_Customers] FOREIGN KEY([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE

)