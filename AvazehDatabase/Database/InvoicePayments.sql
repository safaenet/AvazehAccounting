﻿CREATE TABLE [dbo].[InvoicePayments]
(
    [Id] INT NOT NULL,
    [InvoiceId] INT NOT NULL,
    [DateCreated] CHAR (10) NOT NULL,
    [TimeCreated] CHAR (8) NOT NULL,
    [DateUpdated] CHAR(10) NULL,
    [TimeUpdated] CHAR(8) NULL,
    [PayAmount] BIGINT NOT NULL,
    [Descriptions] NTEXT NULL

    CONSTRAINT [PK_InvoicePayments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InvoicePayments_Invoices] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
)