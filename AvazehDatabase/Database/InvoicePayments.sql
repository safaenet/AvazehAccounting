CREATE TABLE [dbo].[InvoicePayments]
(
    [Id] INT NOT NULL,
    [InvoiceId] INT NOT NULL,
    [DateCreated] [dbo].[DATETYPE] NOT NULL, 
    [TimeCreated] [dbo].[TimeType] NOT NULL, 
    [DateUpdated] [dbo].[DateType] NULL, 
    [TimeUpdated] [dbo].[TimeType] NULL, 
    [PayAmount] [dbo].[CalculationType] NOT NULL,
    [Descriptions] NVARCHAR(MAX) NULL

    CONSTRAINT [PK_InvoicePayments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InvoicePayments_Invoices] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
)