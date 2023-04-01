CREATE TABLE [dbo].[Transactions]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FileName] NVARCHAR(100) NOT NULL,
    [DateCreated] [dbo].[DateType] NOT NULL, 
    [TimeCreated] [dbo].[TimeType] NOT NULL, 
    [DateUpdated] [dbo].[DateType] NULL, 
    [TimeUpdated] [dbo].[TimeType] NULL, 
    [Descriptions] NVARCHAR(MAX) NULL
)