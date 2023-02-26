CREATE TABLE [dbo].[ChequeEvents]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ChequeId] INT NOT NULL,
    [EventDate] DATETIME NOT NULL,
    [EventType] TINYINT NOT NULL,
    [EventText] NVARCHAR(50) NULL

    CONSTRAINT [FK_Cheques_Events] FOREIGN KEY([ChequeId]) REFERENCES [Cheques] ([Id]) ON DELETE CASCADE
)
