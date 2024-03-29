﻿CREATE TABLE [dbo].[Invoices]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [CustomerId] INT NOT NULL, 
    [DateCreated] CHAR(10) NOT NULL, 
    [TimeCreated] CHAR(8) NOT NULL, 
    [DateUpdated] CHAR(10) NULL, 
    [TimeUpdated] CHAR(8) NULL, 
    [DiscountType] TINYINT NULL DEFAULT 0, 
    [DiscountValue] FLOAT NULL DEFAULT 0, 
    [Descriptions] NTEXT NULL, 
    [LifeStatus] TINYINT NOT NULL 
)