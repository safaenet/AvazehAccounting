CREATE TABLE [dbo].[Cheques]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1), 
    [Drawer] NVARCHAR(50) NOT NULL, 
    [Orderer] NVARCHAR(50) NOT NULL, 
    [PayAmount] BIGINT NOT NULL, 
    [About] NVARCHAR(100) NULL, 
    [IssueDate] CHAR(10) NOT NULL, 
    [DueDate] CHAR(10) NOT NULL, 
    [BankName] NVARCHAR(50) NOT NULL, 
    [Serial] NVARCHAR(25) NULL, 
    [Identifier] VARCHAR(20) NULL, 
    [Descriptions] NTEXT NULL
)
