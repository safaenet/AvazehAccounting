CREATE PROCEDURE [dbo].[LoadPrevInvoiceList_OldStyle] 
	@InvoiceId int = -1, --If provided : Search for Invoices.Id<@InvoiceId
	@CustomerId int = -1, --If provided : Search for Invoices.CustomerId=@CustomerId
	@Date dbo.DateType = '%', --If provided : Search for Invoices.DateCreated/DateUpdated=@Date
	@SearchValue NVARCHAR(200) = '%', --If provided : Search for Criterias=@SearchValue
	@OrderType tinyint = 1 --0 = ASC, 1 = DESC
AS

BEGIN

DECLARE @ASC TINYINT = 0
DECLARE @DESC TINYINT = 1

IF(@InvoiceId > 0) SET @CustomerId = (SELECT i.CustomerId FROM Invoices i WHERE i.Id = @InvoiceId)
IF(@SearchValue IS NULL) SET @SearchValue = '%'
IF(@SearchValue <> '%') SET @SearchValue = '%' + TRIM(@SearchValue) + '%'
IF(@Date IS NULL) SET @Date = '%'
IF(@InvoiceId IS NULL) SET @InvoiceId = -1
IF(@CustomerId IS NULL OR @CustomerId = -1) SET @CustomerId = (SELECT CustomerId FROM Invoices WHERE Id = @InvoiceId);

DECLARE @InvoiceSums TABLE --@t1
(
	InvoiceId INT,
	PrevInvoiceId INT,
	PrevInvoiceSum bigint
)

DECLARE @InvoicePayments TABLE --@t2
(
	InvoiceId INT,
	PrevInvoiceId INT,
	PrevInvoicePays bigint
);

INSERT INTO  @InvoiceSums
	SELECT i.Id, i.PrevInvoiceId, CASE WHEN i.DiscountType = 0 THEN SUM(ii.CountValue * ii.SellPrice) - (i.DiscountValue / 100 * SUM(ii.CountValue * ii.SellPrice)) WHEN i.DiscountType = 1 THEN SUM(ii.CountValue * ii.SellPrice) - i.DiscountValue END AS InvoiceSum 
	FROM Invoices i LEFT JOIN InvoiceItems ii ON i.PrevInvoiceId = ii.InvoiceId
	GROUP BY i.Id, i.PrevInvoiceId, i.DiscountType, i.DiscountValue
INSERT INTO @InvoicePayments
	SELECT i.Id, i.PrevInvoiceId, SUM([ip].PayAmount) AS InvoicePays
	FROM Invoices i LEFT JOIN InvoicePayments [ip] ON i.PrevInvoiceId=[ip].InvoiceId
	GROUP BY i.Id, i.PrevInvoiceId;

WITH rCTE(TOPLEVEL, Id, PrevInvoiceId, PrevInvoiceSum, PrevInvoicePays)
AS
(
	SELECT TOPLEVEL=ctePIS.InvoiceId, ctePIS.InvoiceId, ctePIS.PrevInvoiceId, ctePIS.PrevInvoiceSum, ctePIP.PrevInvoicePays
	FROM @InvoiceSums ctePIS INNER JOIN @InvoicePayments ctePIP ON ctePIS.InvoiceId = ctePIP.InvoiceId

	UNION ALL

	SELECT TOPLEVEL=rCTE.TOPLEVEL, [IS].InvoiceId , [IS].PrevInvoiceId, [IS].PrevInvoiceSum, [IP].PrevInvoicePays
	FROM @InvoiceSums [IS] INNER JOIN rCTE ON [IS].InvoiceId = rCTE.PrevInvoiceId INNER JOIN @InvoicePayments [IP] ON [IS].InvoiceId = [IP].InvoiceId
),

PrevInvoiceBalances(InvoiceId, PrevTotalBalance)
AS
(
	SELECT TOPLEVEL, SUM(ISNULL(PrevInvoiceSum, 0)) - SUM(ISNULL(PrevInvoicePays, 0)) 
	FROM rCTE
	GROUP BY TOPLEVEL
)

SELECT	i.Id,
		i.CustomerId,
		ISNULL(cust.FirstName, '') + ' ' + ISNULL(cust.LastName, '') CustomerFullName,
		i.About,
		i.DateCreated,
		i.TimeCreated,
		i.DateUpdated,
		i.TimeUpdated,
		ISNULL(SFS, 0) AS TotalInvoiceSum,
		ISNULL(PT, 0) AS TotalPayments,
		i.Descriptions,
		i.LifeStatus,
		i.PrevInvoiceId,
		ISNULL(pib.PrevTotalBalance, 0) AS PrevInvoiceBalance
		--ISNULL(SFS+pib.PrevTotalRemainedAmount-PT, 0) AS TotalBalance,
		FROM dbo.Invoices i
		INNER JOIN dbo.Customers cust ON i.CustomerId = cust.Id
		LEFT JOIN PrevInvoiceBalances AS pib ON i.Id = pib.InvoiceId,
		(SELECT ROUND(CASE WHEN i.DiscountType = 0 THEN SUM(ii.CountValue * ii.SellPrice) - (i.DiscountValue / 100 * SUM(ii.CountValue * ii.SellPrice)) WHEN i.DiscountType = 1 THEN SUM(ii.CountValue * ii.SellPrice) - i.DiscountValue END, 0) AS SFS, i.Id AS SFN  FROM dbo.InvoiceItems ii RIGHT JOIN dbo.Invoices i ON ii.InvoiceId = i.Id GROUP BY i.Id, i.DiscountType, i.DiscountValue) AS InvSums,
		(SELECT ISNULL(SUM([ip].PayAmount), 0) AS PT, i.Id AS PFN FROM dbo.InvoicePayments [ip] RIGHT JOIN dbo.Invoices i ON [ip].InvoiceId = i.Id GROUP BY i.Id) AS InvPays,
		(SELECT baseI.Id AS FactorNum, prevI.Id AS fwdFactorNum FROM dbo.Invoices baseI left join dbo.Invoices prevI ON baseI.Id = prevI.PrevInvoiceId) AS InvFwds
		WHERE i.Id = InvSums.SFN AND i.Id = InvPays.PFN AND i.Id = InvFwds.FactorNum
		AND (i.LifeStatus = 0)
		AND (i.Id < @InvoiceId)
		AND (i.CustomerId = @CustomerId)
		AND (((@Date <> '%') AND (i.DateCreated LIKE @Date OR i.DateUpdated LIKE @Date)) OR (@Date = '%'))
		AND (SFS + pib.PrevTotalBalance - PT <> 0)
		AND (InvFwds.fwdFactorNum IS NULL)
		AND ((@SearchValue != '%' AND
					(ISNULL(cust.FirstName, '') + ' ' + ISNULL(cust.LastName, '') LIKE @SearchValue OR
					i.About LIKE @SearchValue OR
					i.Descriptions LIKE @SearchValue
					) OR (@SearchValue = '%')))
		ORDER BY CASE WHEN @OrderType = @DESC THEN i.[Id] END DESC, CASE WHEN @OrderType = @ASC THEN i.[Id] END ASC
OPTION (MAXRECURSION 0, RECOMPILE)
	--SET NOCOUNT ON;
 --   DECLARE @ResultTable TABLE
	--	(
	--	[Id] [int],
	--	[CustomerId] [int],
	--	[CustomerFullName] [nvarchar] (102),
	--	[About] [nvarchar] (50),
	--	[DateCreated] [char] (10),
	--	[TimeCreated] [char] (8),
	--	[DateUpdated] [char] (10),
	--	[TimeUpdated] [char] (8),
	--	[TotalInvoiceSum] [decimal],
	--	[TotalPayments] [decimal],
	--	[Descriptions] [ntext],
	--	[LifeStatus] [tinyint],
	--	[PrevInvoiceId] [int],
	--	[PrevInvoiceBalance] [decimal],
	--	[FwdInvoiceId] [int]
	--	)

	--DECLARE @FORWARD TINYINT = 0
	--DECLARE @BACKWARD TINYINT = 1
	--DECLARE @ASC TINYINT = 0
	--DECLARE @DESC TINYINT = 1
	--DECLARE @BALANCED TINYINT = 0
	--DECLARE @DEPTOR TINYINT = 1
	--DECLARE @CREDITOR TINYINT = 2
	--DECLARE @OUTSTANDING TINYINT = 3
	
	--IF(@SearchValue IS NULL) SET @SearchValue = '%'
	--IF(@SearchValue <> '%') SET @SearchValue = '%' + TRIM(@SearchValue) + '%'
	--IF(@Date IS NULL) SET @Date = '%'
	--IF(@InvoiceId IS NULL) SET @InvoiceId = -1
	--IF(@CustomerId IS NULL) SET @CustomerId = -1
	--IF(@LifeStatus IS NULL) SET @LifeStatus = -1
	--IF(@FinStatus IS NULL) SET @FinStatus = -1
	--IF(@SearchMode IS NULL) SET @SearchMode = 1
	--IF(@OrderType IS NULL) SET @OrderType = 1
	--IF(@StartId IS NULL) SET @StartId = -1
	--IF (@SearchMode = @BACKWARD AND @StartId = -1) SET @StartId = (SELECT MAX(Id) + 1 FROM dbo.Invoices);

	--INSERT INTO @ResultTable
	--	SELECT TOP (@FetchSize) i.Id, i.CustomerId, ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') CustomerFullName, i.About, i.DateCreated, i.TimeCreated, i.DateUpdated, i.TimeUpdated, 
	--		dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) AS TotalInvoiceSum, ISNULL(pays.TotalPayments, 0) TotalPayments, i.Descriptions, i.LifeStatus, i.PrevInvoiceId,
	--		dbo.CalculatePrevInvoiceAmount(i.Id) AS PrevInvoiceBalance, fwds.FwdInvoiceId
	--		FROM Invoices i
	--		LEFT JOIN Customers c ON i.CustomerId = c.Id
	--		LEFT JOIN (SELECT SUM(ii.[SellPrice]*ii.[CountValue]) AS TotalSellValue, ii.[InvoiceId] FROM InvoiceItems ii GROUP BY ii.[InvoiceId]) sp ON i.Id = sp.InvoiceId
	--		LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId] FROM InvoicePayments ips GROUP BY ips.[InvoiceId]) pays ON i.Id = pays.InvoiceId
	--		--LEFT JOIN (SELECT inv.Id, [dbo].[CalculatePrevInvoiceAmount](inv.Id) AS [PrevInvoiceBalance] FROM Invoices inv WHERE inv.LifeStatus = 0) prevs ON i.Id = prevs.Id
	--		LEFT JOIN (SELECT baseInv.Id, FwdToInv.Id AS [FwdInvoiceId] FROM Invoices baseInv LEFT JOIN Invoices FwdToInv ON baseInv.Id = FwdToInv.PrevInvoiceId WHERE baseInv.LifeStatus = 0 AND FwdToInv.Id IS NOT NULL) fwds ON i.Id = fwds.Id
	--		WHERE ((@LifeStatus != -1 AND i.LifeStatus = @LifeStatus) OR (@LifeStatus = -1))
	--		AND ((@InvoiceId > -1 AND (i.Id = @InvoiceId OR i.[PrevInvoiceId] = @InvoiceId OR fwds.FwdInvoiceId = @InvoiceId)) OR (@InvoiceId = -1))
	--		AND ((@CustomerId > -1 AND i.CustomerId = @CustomerId) OR (@CustomerId = -1))
	--		AND ((@SearchMode = @BACKWARD AND i.Id < @StartId) OR (@SearchMode = @FORWARD AND i.Id > @StartId))
	--		AND (((@Date <> '%') AND (i.DateCreated LIKE @Date OR i.DateUpdated LIKE @Date)) OR (@Date = '%'))
	--		AND ((@SearchValue != '%' AND
	--				(ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') LIKE @SearchValue OR
	--				i.About LIKE @SearchValue OR
	--				i.Descriptions LIKE @SearchValue
	--				) OR (@SearchValue = '%')))
	--		AND ((@FinStatus <> -1 AND ((@FinStatus = @BALANCED AND (dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue)-ISNULL(pays.TotalPayments, 0)+dbo.CalculatePrevInvoiceAmount(i.Id)) = 0) OR 
	--			 (@FinStatus = @DEPTOR AND (dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue)-ISNULL(pays.TotalPayments, 0)+dbo.CalculatePrevInvoiceAmount(i.Id)) > 0) OR
	--			 (@FinStatus = @CREDITOR AND (dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue)-ISNULL(pays.TotalPayments, 0)+dbo.CalculatePrevInvoiceAmount(i.Id)) < 0) OR
	--			 (@FinStatus = @OUTSTANDING AND fwds.FwdInvoiceId IS NULL AND (dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue)-ISNULL(pays.TotalPayments, 0)+dbo.CalculatePrevInvoiceAmount(i.Id)) <> 0)) OR
	--			  @FinStatus = -1)
	--		)
	--		ORDER BY CASE WHEN @SearchMode = @BACKWARD THEN i.[Id] END DESC, CASE WHEN @SearchMode = @FORWARD THEN i.[Id] END ASC
			
	--SELECT rt.* FROM @ResultTable rt ORDER BY CASE WHEN @OrderType = @DESC THEN rt.[Id] END DESC, CASE WHEN @OrderType = @ASC THEN rt.[Id] END ASC
	--OPTION(maxrecursion 32767, recompile);
	
END
GO