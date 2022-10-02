﻿CREATE TABLE [dbo].[UserSettings]
(
    [Id] INT NOT NULL PRIMARY KEY, 
    [ColorNewItem] NVARCHAR(10) NOT NULL DEFAULT '#FFF5F533', 
    [ColorSoldItem] NVARCHAR(10) NOT NULL DEFAULT '#ff66e6ff', 
    [ColorNonSufficientFundItem] NVARCHAR(10) NOT NULL DEFAULT '#ffff9c7a', 
    [ColorCashedItem] NVARCHAR(10) NOT NULL DEFAULT '#ff94ffb6', 
    [ColorChequeNotification] NVARCHAR(10) NOT NULL DEFAULT '#fff4ff8c', 
    [ColorUpdatedItem] NVARCHAR(10) NOT NULL DEFAULT '#FFDEDEDE', 
    [ColorBalancedItem] NVARCHAR(10) NOT NULL DEFAULT '#ff94ffb6', 
    [ColorDeptorItem] NVARCHAR(10) NOT NULL DEFAULT '#ffff9c7a', 
    [ColorCreditorItem] NVARCHAR(10) NOT NULL DEFAULT '#ff7ad3ff', 
    [ColorInactiveItem] NVARCHAR(10) NOT NULL DEFAULT '#ffc9c9c9', 
    [ColorArchivedItem] NVARCHAR(10) NOT NULL DEFAULT '#ffffe0a3', 
    [ColorDeletedItem] NVARCHAR(10) NOT NULL DEFAULT '#ffff6b6b', 
    [ColorNegativeProfit] NVARCHAR(10) NOT NULL DEFAULT '#ffffadad',
    [ColorPositiveItem] NVARCHAR(10) NOT NULL DEFAULT '#ff7ad3ff', 
    [ColorNegativeItem] NVARCHAR(10) NOT NULL DEFAULT '#ffff9c7a',
    [DataGridFontSize] SMALLINT NOT NULL DEFAULT 12, 
    [ChequeListPageSize] INT NOT NULL DEFAULT 50, 
    [ChequeListQueryOrderType] NVARCHAR(5) NOT NULL DEFAULT 'DESC', 
    [ChequeNotifyDays] SMALLINT NOT NULL DEFAULT 2, 
    [ChequeNotify] BIT NOT NULL DEFAULT 1, 
    [InvoicePageSize] INT NOT NULL DEFAULT 50, 
    [InvoiceListQueryOrderType] NVARCHAR(5) NOT NULL DEFAULT 'DESC', 
    [InvoiceDetailQueryOrderType] NVARCHAR(5) NOT NULL DEFAULT 'DESC', 
    [TransactionListPageSize] INT NOT NULL DEFAULT 50, 
    [TransactionDetailPageSize] INT NOT NULL DEFAULT 50, 
    [TransactionListQueryOrderType] NVARCHAR(5) NOT NULL DEFAULT 'DESC', 
    [TransactionDetailQueryOrderType] NVARCHAR(5) NOT NULL DEFAULT 'DESC', 

    [AutoSelectPersianLanguage] BIT NOT NULL DEFAULT 0, 
    [TransactionShortcut1Id] INT NOT NULL DEFAULT 0, 
    [TransactionShortcut2Id] INT NOT NULL DEFAULT 0, 
    [TransactionShortcut3Id] INT NOT NULL DEFAULT 0, 
    [TransactionShortcut1Name] NVARCHAR(50) NULL DEFAULT 'میانبر یک', 
    [TransactionShortcut2Name] NVARCHAR(50) NULL DEFAULT 'میانبر دو', 
    [TransactionShortcut3Name] NVARCHAR(50) NULL DEFAULT 'میانبر سه', 
    [AskToAddNotExistingProduct] BIT NOT NULL DEFAULT 1, 
)
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'When product doesn''t exist when adding to invoice list, ask for adding new one.',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'UserSettings',
    @level2type = N'COLUMN',
    @level2name = N'AskToAddNotExistingProduct'