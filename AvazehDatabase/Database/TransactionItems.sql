CREATE TABLE [dbo].[TransactionItems]
(
	[Id] INT NOT NULL PRIMARY KEY, 
	[TransactionId] INT NOT NULL, 
    [Title] NVARCHAR(100) NOT NULL, 
    [Amount] BIGINT NOT NULL,
	[CountString] NVARCHAR(50) NOT NULL,
    [CountValue] [dbo].[CountType] NOT NULL,
    [DateCreated] [dbo].[DateType] NOT NULL, 
    [TimeCreated] [dbo].[TimeType] NOT NULL, 
    [DateUpdated] [dbo].[DateType] NULL, 
    [TimeUpdated] [dbo].[TimeType] NULL, 
    [Descriptions] NVARCHAR(50) NULL

    CONSTRAINT [FK_Transactions_TransactionItems] FOREIGN KEY([TransactionId]) REFERENCES [Transactions] ([Id]) ON DELETE CASCADE
)
