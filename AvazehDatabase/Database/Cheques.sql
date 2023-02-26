CREATE TABLE [dbo].[Cheques]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Drawer] NVARCHAR(50) NOT NULL, 
    [Orderer] NVARCHAR(50) NOT NULL, 
    [PayAmount] BIGINT NOT NULL, 
    [About] NVARCHAR(100) NULL, 
    [IssueDate] DATETIME NOT NULL, 
    [DueDate] DATETIME NOT NULL, 
    [BankName] NVARCHAR(50) NOT NULL, 
    [Serial] NVARCHAR(25) NULL, 
    [Identifier] NVARCHAR(20) NULL, 
    [Descriptions] NTEXT NULL
)
