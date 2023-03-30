CREATE PROCEDURE [dbo].[LoadSingleInvoiceDetails]
	@InvoiceId INT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @invoice TABLE
	(
		[Id] [int],
		[CustomerId] [int],
		[About] [nvarchar] (50),
		[DateCreated] [char] (10),
		[TimeCreated] [char] (8),
		[DateUpdated] [char] (10),
		[TimeUpdated] [char] (8),
		[DiscountType] [tinyint],
		[DiscountValue] [float],
		[Descriptions] [ntext],
		[LifeStatus] [tinyint],
		[PrevInvoiceId] [int],
		[PrevInvoiceBalance] [float],
		
		[CustId] [int],
		[FirstName] [nvarchar](50),
		[LastName] [nvarchar](50),
		[CompanyName] [nvarchar](50),
		[EmailAddress] [nvarchar](50),
		[PostAddress] [ntext],
		[DateJoined] [char](10),
		[CustDescriptions] [ntext]
	)
		INSERT INTO @invoice
		SELECT i.Id, i.CustomerId, i.About, i.DateCreated, i.TimeCreated, i.DateUpdated, i.TimeUpdated, i.DiscountType, i.DiscountValue, i.Descriptions, i.LifeStatus, i.PrevInvoiceId, [dbo].[CalculatePrevInvoiceAmount](@InvoiceId) AS [PrevInvoiceBalance],
			c.Id as CustId, FirstName, LastName, CompanyName, EmailAddress, PostAddress, DateJoined, c.Descriptions as CustDescriptions
            FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id
			WHERE i.Id = @InvoiceId

			SELECT * FROM @invoice ORDER BY [Id] ASC;
            SELECT it.Id, it.InvoiceId, it.BuyPrice, it.SellPrice, it.CountString, it.DateCreated, it.TimeCreated, it.DateUpdated, it.DateUpdated, it.TimeUpdated, it.Delivered, it.Descriptions,
                p.Id pId, p.ProductName, p.BuyPrice pBuyPrice, p.SellPrice pSellPrice, p.Barcode, p.CountString pCountString, p.DateCreated pDateCreated, p.TimeCreated pTimeCreated,
                p.DateUpdated pDateUpdated, p.TimeUpdated pTimeUpdated, p.Descriptions pDescriptions, u.Id AS puId, u.UnitName
                FROM InvoiceItems it
				LEFT JOIN Products p ON it.ProductId = p.Id
				LEFT JOIN ProductUnits u ON it.ProductUnitId = u.Id
				WHERE it.InvoiceId IN (SELECT i.Id FROM @invoice i) ORDER BY it.[Id] DESC;
            SELECT * FROM InvoicePayments WHERE InvoiceId IN (SELECT i.Id FROM @invoice i);
            SELECT * FROM PhoneNumbers WHERE CustomerId IN (SELECT i.CustomerId FROM @invoice i);
END
GO