CREATE TABLE [dbo].[TransactionItems]
(
	[Id] INT NOT NULL PRIMARY KEY, 
	[TransactionId] INT NOT NULL, 
    [Title] NVARCHAR(100) NOT NULL, 
    [Amount] BIGINT NOT NULL,
	[CountString] NVARCHAR(50) NOT NULL,
    [CountValue] FLOAT NOT NULL,
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL, 
    [Descriptions] NVARCHAR(50) NULL

    CONSTRAINT [FK_Transactions_TransactionItems] FOREIGN KEY([TransactionId]) REFERENCES [Transactions] ([Id]) ON DELETE CASCADE
)
