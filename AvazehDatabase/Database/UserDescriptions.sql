CREATE TABLE [dbo].[UserDescriptions]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [DescriptionTitle] NVARCHAR(30) NULL, 
    [DescriptionText] NVARCHAR(MAX) NOT NULL
)
