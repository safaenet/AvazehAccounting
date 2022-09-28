CREATE TABLE [dbo].[UserInfo]
(
    [Username] NVARCHAR(50) NOT NULL, 
    [PasswordHash] VARBINARY(MAX) NOT NULL, 
    [PasswordSalt] VARBINARY(MAX) NOT NULL, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NULL, 
    [DateCreated] CHAR(10) NOT NULL, 
    [LastLoginDate] CHAR(10) NULL, 
    CONSTRAINT [PK_UserInfo] PRIMARY KEY ([Username]), 
    CONSTRAINT [FK_UserInfo_ToUserPermissions] FOREIGN KEY ([Username]) REFERENCES [UserPermissions]([Username]),
    CONSTRAINT [FK_UserInfo_ToUserSettings] FOREIGN KEY ([Username]) REFERENCES [UserSettings]([Username])
)