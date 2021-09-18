CREATE TABLE [dbo].[Customers]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NULL, 
	[CompanyName] NVARCHAR(50) NULL,
    [EmailAddress] NVARCHAR(50) NULL, 
    [PostAddress] NTEXT NULL, 
    [DateJoined] CHAR(10) NULL, 
    [Descriptions] NTEXT NULL
    
	CHECK ([EmailAddress] LIKE '%_@_%._%'), 
)