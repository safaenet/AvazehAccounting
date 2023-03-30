CREATE PROCEDURE [dbo].[LoadCountOfInvoiceList] 
	@InvoiceId int = -1,
	@CustomerId int = -1,
	@Date char(10) = '%',
	@SearchValue nvarchar(200) = '%',
	@LifeStatus smallint = -1,
	@FinStatus smallint = -1
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @BALANCED TINYINT = 0
	DECLARE @DEPTOR TINYINT = 1
	DECLARE @CREDITOR TINYINT = 2
	DECLARE @OUTSTANDING TINYINT = 3
	
	IF(@SearchValue IS NULL) SET @SearchValue = '%'
	IF(@SearchValue <> '%') SET @SearchValue = '%' + TRIM(@SearchValue) + '%'
	IF(@Date IS NULL) SET @Date = '%'
	IF(@InvoiceId IS NULL) SET @InvoiceId = -1
	IF(@CustomerId IS NULL) SET @CustomerId = -1
	IF(@LifeStatus IS NULL) SET @LifeStatus = -1
	IF(@FinStatus IS NULL) SET @FinStatus = -1

		SELECT COUNT(i.Id)
			FROM Invoices i
			LEFT JOIN Customers c ON i.CustomerId = c.Id
			LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.SellPrice) AS TotalSellValue, ii.[InvoiceId] FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id = sp.InvoiceId
			LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId] FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id = pays.InvoiceId
			LEFT JOIN (SELECT inv.Id, [dbo].[CalculatePrevInvoiceAmount](inv.Id) AS [PrevInvoiceBalance] FROM Invoices inv WHERE inv.LifeStatus = 0) prevs ON i.Id = prevs.Id
			LEFT JOIN (SELECT baseInv.Id, FwdToInv.Id AS [FwdInvoiceId] FROM Invoices baseInv LEFT JOIN Invoices FwdToInv ON baseInv.Id = FwdToInv.PrevInvoiceId WHERE baseInv.LifeStatus = 0 AND FwdToInv.Id IS NOT NULL) fwds ON i.Id = fwds.Id
			WHERE ((@LifeStatus != -1 AND i.LifeStatus = @LifeStatus) OR (@LifeStatus = -1))
			AND ((@InvoiceId > -1 AND i.Id = @InvoiceId) OR (@InvoiceId = -1))
			AND ((@CustomerId > -1 AND i.CustomerId = @CustomerId) OR (@CustomerId = -1))
			AND (((@Date <> '%') AND (i.DateCreated LIKE @Date OR i.DateUpdated LIKE @Date)) OR (@Date = '%'))
			AND ((@SearchValue != '%' AND
					(ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') LIKE @SearchValue OR
					i.About LIKE @SearchValue OR
					[TimeCreated] LIKE @SearchValue OR [TimeUpdated] LIKE @SearchValue OR
					dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) LIKE @SearchValue OR
					ISNULL(pays.TotalPayments, 0) LIKE @SearchValue OR
					dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) - ISNULL(pays.TotalPayments, 0) LIKE @SearchValue OR
					dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) - ISNULL(pays.TotalPayments, 0) + ISNULL(prevs.PrevInvoiceBalance, 0) LIKE @SearchValue OR
					i.Descriptions LIKE @SearchValue OR
					(CAST(ISNULL(prevs.PrevInvoiceBalance, 0) AS VARCHAR) LIKE @SearchValue OR CAST((dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue)-ISNULL(pays.TotalPayments, 0)) AS VARCHAR) LIKE @SearchValue OR CAST((dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue)-ISNULL(pays.TotalPayments, 0)+ISNULL(prevs.PrevInvoiceBalance, 0)) AS VARCHAR) LIKE @SearchValue OR
					CAST(fwds.FwdInvoiceId AS VARCHAR) LIKE @SearchValue) OR
					i.[PrevInvoiceId] LIKE @SearchValue) OR (@SearchValue = '%'))
				)
			AND ((@FinStatus <> -1 AND ((@FinStatus = @BALANCED AND (dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue)-ISNULL(pays.TotalPayments, 0)+ISNULL(prevs.PrevInvoiceBalance, 0)) = 0) OR 
				 (@FinStatus = @DEPTOR AND (dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue)-ISNULL(pays.TotalPayments, 0)+ISNULL(prevs.PrevInvoiceBalance, 0)) > 0) OR
				 (@FinStatus = @CREDITOR AND (dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue)-ISNULL(pays.TotalPayments, 0)+ISNULL(prevs.PrevInvoiceBalance, 0)) < 0) OR
				 (@FinStatus = @OUTSTANDING AND fwds.FwdInvoiceId IS NULL AND (dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue)-ISNULL(pays.TotalPayments, 0)+ISNULL(prevs.PrevInvoiceBalance, 0)) <> 0)) OR
				  @FinStatus = -1)
			)
END
GO