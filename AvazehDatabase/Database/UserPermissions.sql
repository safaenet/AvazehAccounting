﻿CREATE TABLE [dbo].[UserPermissions]
(
    [Id] INT NOT NULL PRIMARY KEY, 
    [CanViewCustomersList] BIT NOT NULL DEFAULT 1, 
    [CanViewCustomerDetails] BIT NOT NULL DEFAULT 1, 
    [CanViewProductsList] BIT NOT NULL DEFAULT 1, 
    [CanViewProductDetails] BIT NOT NULL DEFAULT 1, 
    [CanViewInvoicesList] BIT NOT NULL DEFAULT 1, 
    [CanViewInvoiceDetails] BIT NOT NULL DEFAULT 1, 
    [CanViewTransactionsList] BIT NOT NULL DEFAULT 1, 
    [CanViewTransactionDetails] BIT NOT NULL DEFAULT 1, 
    [CanViewChequesList] BIT NOT NULL DEFAULT 1, 
    [CanViewChequeDetails] BIT NOT NULL DEFAULT 1, 

    [CanAddNewCustomer] BIT NOT NULL DEFAULT 1, 
    [CanAddNewProduct] BIT NOT NULL DEFAULT 1, 
    [CanAddNewInvoice] BIT NOT NULL DEFAULT 1, 
    [CanAddNewTransaction] BIT NOT NULL DEFAULT 1, 
    [CanAddNewCheque] BIT NOT NULL DEFAULT 1, 

    [CanEditCustomer] BIT NOT NULL DEFAULT 1, 
    [CanEditProduct] BIT NOT NULL DEFAULT 1, 
    [CanEditInvoice] BIT NOT NULL DEFAULT 1, 
    [CanEditTransaction] BIT NOT NULL DEFAULT 1, 
    [CanEditCheque] BIT NOT NULL DEFAULT 1, 

    [CanDeleteCustomer] BIT NOT NULL DEFAULT 1, 
    [CanDeleteProduct] BIT NOT NULL DEFAULT 1, 
    [CanDeleteInvoice] BIT NOT NULL DEFAULT 1, 
    [CanDeleteInvoiceItem] BIT NOT NULL DEFAULT 1, 
    [CanDeleteTransaction] BIT NOT NULL DEFAULT 1, 
    [CanDeleteTransactionItem] BIT NOT NULL DEFAULT 1, 
    [CanDeleteCheque] BIT NOT NULL DEFAULT 1, 

    [CanPrintInvoice] BIT NOT NULL DEFAULT 1, 
    [CanPrintTransaction] BIT NOT NULL DEFAULT 1, 
    [CanViewNetProfits] BIT NOT NULL DEFAULT 1, 
    [CanUseBarcodeReader] BIT NOT NULL DEFAULT 1, 
    [CanManageItself] BIT NOT NULL DEFAULT 1, 
    [CanManageOthers] BIT NOT NULL DEFAULT 1, 
    
    CONSTRAINT [FK_UserInfo_ToUserPermissions] FOREIGN KEY ([Id]) REFERENCES [UserInfo]([Id]) ON DELETE CASCADE,
)