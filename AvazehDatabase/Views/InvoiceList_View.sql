SELECT i.Id, i.CustomerId, ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') AS CustomerFullName, i.About, i.DateCreated, i.TimeCreated, i.DateUpdated, i.TimeUpdated,
    dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) AS TotalInvoiceSum, ISNULL(pays.TotalPayments, 0) AS TotalPayments,
    dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, sp.TotalSellValue) - ISNULL(pays.TotalPayments, 0) + ISNULL(prevs.PrevInvoiceBalance, 0) AS InvoiceTotalBalance,
    i.Descriptions, i.LifeStatus, i.PrevInvoiceId, prevs.PrevInvoiceBalance, fwds.FwdInvoiceId
FROM dbo.Invoices AS i LEFT OUTER JOIN
                         dbo.Customers AS c ON i.CustomerId = c.Id LEFT OUTER JOIN
                             (SELECT        SUM([CountValue] * SellPrice) AS TotalSellValue, InvoiceId
                                FROM            dbo.InvoiceItems AS ii
                                GROUP BY InvoiceId) AS sp ON i.Id = sp.InvoiceId LEFT OUTER JOIN
                             (SELECT        SUM(PayAmount) AS TotalPayments, InvoiceId
                                FROM            dbo.InvoicePayments AS ips
                                GROUP BY InvoiceId) AS pays ON i.Id = pays.InvoiceId LEFT OUTER JOIN
                             (SELECT        Id, dbo.CalculatePrevInvoiceAmount(Id) AS PrevInvoiceBalance
                                FROM            dbo.Invoices AS inv
                                WHERE        (LifeStatus = 0)) AS prevs ON i.Id = prevs.Id LEFT OUTER JOIN
                             (SELECT        baseInv.Id, FwdToInv.Id AS FwdInvoiceId
                                FROM            dbo.Invoices AS baseInv LEFT OUTER JOIN
                                                         dbo.Invoices AS FwdToInv ON baseInv.Id = FwdToInv.PrevInvoiceId
                                WHERE        (baseInv.LifeStatus = 0) AND (FwdToInv.Id IS NOT NULL)) AS fwds ON i.Id = fwds.Id