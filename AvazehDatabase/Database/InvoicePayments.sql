CREATE TABLE [dbo].[InvoicePayments]
(
    [Id] INT NOT NULL,
    [InvoiceId] INT NOT NULL,
    [DateCreated] DATETIME NOT NULL,
    [DateUpdated] DATETIME NULL,
    [PayAmount] BIGINT NOT NULL,
    [Descriptions] NTEXT NULL

    CONSTRAINT [PK_InvoicePayments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InvoicePayments_Invoices] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
)