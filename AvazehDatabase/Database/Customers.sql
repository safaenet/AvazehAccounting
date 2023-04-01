CREATE TABLE [dbo].[Customers]
(
	[Id] INT NOT NULL PRIMARY KEY,
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NULL, 
	[CompanyName] NVARCHAR(50) NULL,
    [EmailAddress] NVARCHAR(50) NULL, 
    [PostAddress] NVARCHAR(MAX) NULL, 
    [DateJoined] [dbo].[DateType] NOT NULL, 
    [Descriptions] NVARCHAR(MAX) NULL
    
	CHECK ([EmailAddress] LIKE '%_@_%._%'), 
)