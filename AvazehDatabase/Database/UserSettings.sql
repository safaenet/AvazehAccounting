CREATE TABLE [dbo].[UserSettings]
(
	[Username] NVARCHAR(50) NOT NULL, 
    [DataGridFontSize] SMALLINT NOT NULL DEFAULT 12, 
    [DataGridNewItemColor] NVARCHAR(10) NOT NULL DEFAULT '#FFF5F533', 
    [ChequeListPageSize] NCHAR(10) NOT NULL DEFAULT 50, 
    [ChequeListQueryOrderType] NVARCHAR(5) NOT NULL DEFAULT 'DESC', 
    CONSTRAINT [PK_UserSettings] PRIMARY KEY ([Username]), 
)