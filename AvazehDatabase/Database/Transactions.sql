CREATE TABLE [dbo].[Transactions]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FileName] NVARCHAR(100) NOT NULL,
	[DateCreated] CHAR(10) NOT NULL, 
    [TimeCreated] CHAR(8) NOT NULL, 
    [DateUpdated] CHAR(10) NULL, 
    [TimeUpdated] CHAR(8) NULL, 
    [Descriptions] NTEXT NULL
)