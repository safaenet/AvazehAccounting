﻿CREATE TABLE [dbo].[TransactionItems]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1), 
	[TransactionId] INT NOT NULL, 
    [Title] NVARCHAR(100) NOT NULL, 
    [Amount] NCHAR(10) NOT NULL,
	[CountString] NVARCHAR(50) NOT NULL,
    [CountValue] FLOAT NOT NULL,
    [DateCreated] CHAR(10) NOT NULL, 
    [TimeCreated] CHAR(8) NOT NULL, 
    [DateUpdated] CHAR(10) NULL, 
    [TimeUpdated] CHAR(8) NULL, 
    [Descriptions] NVARCHAR(50) NULL

    CONSTRAINT [FK_Transactions_TransactionItems] FOREIGN KEY([TransactionId]) REFERENCES [Transactions] ([Id]) ON DELETE CASCADE
)
