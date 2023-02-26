CREATE TABLE [dbo].[Transactions]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FileName] NVARCHAR(100) NOT NULL,
	[DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL, 
    [Descriptions] NTEXT NULL
)