CREATE PROCEDURE [dbo].[LoadInvoiceList] 
	@FetchSize int = 50,
	@InvoiceId int = -1,
	@CustomerId int = -1,
	@Date char(10) = '%',
	@SearchValue nvarchar(200) = '%',
	@LifeStatus smallint = -1,
	@FinStatus smallint = -1,
	@SearchMode tinyint = 1, --'0' = Forward, '1' = Backward
	@OrderType tinyint = 1, --0 = ASC, 1 = DESC
	@StartId int = -1
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @ResultTable TABLE
		(
		[Id] [int],
		[CustomerId] [int],
		[CustomerFullName] [nvarchar] (102),
		[About] [nvarchar] (50),
		[DateCreated] [char] (10),
		[TimeCreated] [char] (8),
		[DateUpdated] [char] (10),
		[TimeUpdated] [char] (8),
		[TotalInvoiceSum] [decimal],
		[TotalPayments] [decimal],
		[Descriptions] [ntext],
		[LifeStatus] [tinyint],
		[PrevInvoiceId] [int],
		[PrevInvoiceBalance] [decimal],
		[FwdInvoiceId] [int]
		)

	DECLARE @FORWARD TINYINT = 0
	DECLARE @BACKWARD TINYINT = 1
	DECLARE @ASC TINYINT = 0
	DECLARE @DESC TINYINT = 1
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
	IF(@SearchMode IS NULL) SET @SearchMode = 1
	IF(@OrderType IS NULL) SET @OrderType = 1
	IF(@StartId IS NULL) SET @StartId = -1
	IF (@SearchMode = @BACKWARD AND @StartId = -1) SET @StartId = (SELECT MAX(Id) + 1 FROM dbo.Invoices);

	INSERT INTO @ResultTable
		SELECT TOP (@FetchSize) i.Id, i.CustomerId, ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') CustomerFullName, i.About, i.DateCreated, i.TimeCreated, i.DateUpdated, i.TimeUpdated, 
			dbo.GetDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) AS TotalInvoiceSum, ISNULL(pays.TotalPayments, 0) TotalPayments, i.Descriptions, i.LifeStatus, i.PrevInvoiceId,
			prevs.PrevInvoiceBalance, fwds.FwdInvoiceId
			FROM Invoices i
			LEFT JOIN Customers c ON i.CustomerId = c.Id
			LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.SellPrice) AS TotalSellValue, ii.[InvoiceId] FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id = sp.InvoiceId
			LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId] FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id = pays.InvoiceId
			LEFT JOIN (SELECT inv.Id, [dbo].[CalculatePrevInvoiceAmount](inv.Id) AS [PrevInvoiceBalance] FROM Invoices inv WHERE inv.LifeStatus = 0) prevs ON i.Id = prevs.Id
			LEFT JOIN (SELECT baseInv.Id, FwdToInv.Id AS [FwdInvoiceId] FROM Invoices baseInv LEFT JOIN Invoices FwdToInv ON baseInv.Id = FwdToInv.PrevInvoiceId WHERE baseInv.LifeStatus = 0 AND FwdToInv.Id IS NOT NULL) fwds ON i.Id = fwds.Id
			WHERE ((@LifeStatus != -1 AND i.LifeStatus = @LifeStatus) OR (@LifeStatus = -1))
			AND ((@InvoiceId > -1 AND i.Id = @InvoiceId) OR (@InvoiceId = -1))
			AND ((@CustomerId > -1 AND i.CustomerId = @CustomerId) OR (@CustomerId = -1))
			AND ((@SearchMode = @BACKWARD AND i.Id < @StartId) OR (@SearchMode = @FORWARD AND i.Id > @StartId))
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
			ORDER BY CASE WHEN @SearchMode = @BACKWARD THEN i.[Id] END DESC, CASE WHEN @SearchMode = @FORWARD THEN i.[Id] END ASC
	SELECT rt.* FROM @ResultTable rt ORDER BY CASE WHEN @OrderType = @DESC THEN rt.[Id] END DESC, CASE WHEN @OrderType = @ASC THEN rt.[Id] END ASC
END
GO