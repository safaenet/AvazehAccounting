CREATE TABLE [dbo].[UserDescriptions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DescriptionTitle] NVARCHAR(30) NULL, 
    [DescriptionText] NVARCHAR(MAX) NOT NULL
)
