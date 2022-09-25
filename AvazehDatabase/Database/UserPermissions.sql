CREATE TABLE [dbo].[UserPermissions]
(
	[Username] NVARCHAR(50) NOT NULL, 
    [CanViewCustomers] BIT NOT NULL DEFAULT 1, 
    [CanViewProducts] BIT NOT NULL DEFAULT 1, 
    [CanViewInvoicesList] BIT NOT NULL DEFAULT 1, 
    [CanViewInvoiceDetails] BIT NOT NULL DEFAULT 1, 
    [CanViewTransactionsList] BIT NOT NULL DEFAULT 1, 
    [CanViewTransactionDetails] BIT NOT NULL DEFAULT 1, 
    [CanViewCheques] BIT NOT NULL DEFAULT 1, 

    [CanAddNewCustomer] BIT NOT NULL DEFAULT 1, 
    [CanAddNewProduct] BIT NOT NULL DEFAULT 1, 
    [CanAddNewInvoice] BIT NOT NULL DEFAULT 1, 
    [CanAddNewTransaction] BIT NOT NULL DEFAULT 1, 
    [CanAddNewCheque] BIT NOT NULL DEFAULT 1, 

    [CanEditCustomers] BIT NOT NULL DEFAULT 1, 
    [CanEditProducts] BIT NOT NULL DEFAULT 1, 
    [CanEditInvoices] BIT NOT NULL DEFAULT 1, 
    [CanEditTransactions] BIT NOT NULL DEFAULT 1, 
    [CanEditCheques] BIT NOT NULL DEFAULT 1, 

    [CanPrintInvoice] BIT NOT NULL DEFAULT 1, 
    [CanPrintTransaction] BIT NOT NULL DEFAULT 1, 
    [CanChangeItsSettings] BIT NOT NULL DEFAULT 1, 
    [CanChangePassword] BIT NOT NULL DEFAULT 1, 
    [CanAddUser] BIT NOT NULL DEFAULT 0, 
    [CanEditUsers] BIT NOT NULL DEFAULT 0, 
    [CanViewNetProfits] BIT NOT NULL DEFAULT 0, 
    [CanUseBarcodeReader] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([Username]), 
)