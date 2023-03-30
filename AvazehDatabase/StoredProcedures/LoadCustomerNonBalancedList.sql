CREATE PROCEDURE [dbo].[LoadCustomerNonBalancedList] 
	@InvoiceId int = -1, --If provided : Search for Invoices.Id<@InvoiceId
	@CustomerId int = -1, --If provided : Search for Invoices.CustomerId=@CustomerId
	@Date CHAR(10) = '%', --If provided : Search for Invoices.DateCreated/DateUpdated=@Date
	@SearchValue NVARCHAR(200) = '%', --If provided : Search for Criterias=@SearchValue
	@OrderType tinyint = 1 --0 = ASC, 1 = DESC
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ASC TINYINT = 0
	DECLARE @DESC TINYINT = 1

	IF(@InvoiceId > 0) SET @CustomerId = (SELECT i.CustomerId FROM Invoices i WHERE i.Id = @InvoiceId)
	IF(@SearchValue IS NULL) SET @SearchValue = '%'
	IF(@SearchValue <> '%') SET @SearchValue = '%' + TRIM(@SearchValue) + '%'
	IF(@Date IS NULL) SET @Date = '%'
	IF(@InvoiceId IS NULL) SET @InvoiceId = -1
	IF(@CustomerId IS NULL) SET @CustomerId = -1

	SELECT i.Id, i.CustomerId, ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') CustomerFullName, i.About, i.DateCreated, i.TimeCreated, i.DateUpdated, i.TimeUpdated,
		dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) AS TotalInvoiceSum, ISNULL(pays.TotalPayments, 0) TotalPayments, i.Descriptions, i.LifeStatus, i.PrevInvoiceId,
		prevs.PrevInvoiceBalance, fwds.FwdInvoiceId
		FROM Invoices i
		LEFT JOIN Customers c ON i.CustomerId = c.Id
		LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.SellPrice) AS TotalSellValue, ii.[InvoiceId] FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id = sp.InvoiceId
		LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId] FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id = pays.InvoiceId
		LEFT JOIN (SELECT inv.Id, [dbo].[CalculatePrevInvoiceAmount](inv.Id) AS [PrevInvoiceBalance] FROM Invoices inv WHERE inv.LifeStatus = 0) prevs ON i.Id = prevs.Id
			LEFT JOIN (SELECT baseInv.Id, FwdToInv.Id AS [FwdInvoiceId] FROM Invoices baseInv LEFT JOIN Invoices FwdToInv ON baseInv.Id = FwdToInv.PrevInvoiceId WHERE baseInv.LifeStatus = 0 AND FwdToInv.Id IS NOT NULL) fwds ON i.Id = fwds.Id
		WHERE (i.LifeStatus = 0)
		AND ((@InvoiceId > -1 AND i.Id < @InvoiceId) OR (@InvoiceId = -1))
		AND ((@CustomerId > -1 AND i.CustomerId = @CustomerId) OR (@CustomerId = -1))
		AND (((@Date <> '%') AND (i.DateCreated LIKE @Date OR i.DateUpdated LIKE @Date)) OR (@Date = '%'))
		AND (dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) - ISNULL(pays.TotalPayments, 0) <> 0)
		AND (fwds.FwdInvoiceId IS NULL)
		AND ((@SearchValue != '%' AND
					(ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') LIKE @SearchValue OR
					i.About LIKE @SearchValue OR
					[TimeCreated] LIKE @SearchValue OR [TimeUpdated] LIKE @SearchValue OR
					dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) LIKE @SearchValue OR
					ISNULL(pays.TotalPayments, 0) LIKE @SearchValue OR
					dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) - ISNULL(pays.TotalPayments, 0) LIKE @SearchValue OR
					dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) - ISNULL(pays.TotalPayments, 0) + ISNULL(prevs.PrevInvoiceBalance, 0) LIKE @SearchValue OR
					i.Descriptions LIKE @SearchValue OR
					i.[PrevInvoiceId] LIKE @SearchValue) OR (@SearchValue = '%'))
				)
		ORDER BY CASE WHEN @OrderType = @DESC THEN i.[Id] END DESC, CASE WHEN @OrderType = @ASC THEN i.[Id] END ASC
END
GO