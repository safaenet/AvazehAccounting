CREATE TABLE [dbo].[UserInfo]
(
    [Username] NVARCHAR(50) NOT NULL, 
    [PasswordHash] NVARCHAR(256) NOT NULL, 
    [PasswordSalt] NVARCHAR(50) NOT NULL, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_UserInfo] PRIMARY KEY ([Username]) 
)