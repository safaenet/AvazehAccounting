CREATE FUNCTION [dbo].[CalculateInvoiceBalance] (@InvoiceId INT)
RETURNS DECIMAL
AS
BEGIN
	DECLARE @Result DECIMAL = (SELECT dbo.CalculateDiscountedInvoiceSum(i.DiscountType, i.DiscountValue, ISNULL(sp.TotalSellValue, 0)) - ISNULL(pays.TotalPayments, 0) FROM Invoices i
			LEFT JOIN (SELECT SUM(ii.[CountValue] * ii.SellPrice) AS TotalSellValue, ii.[InvoiceId] FROM InvoiceItems ii WHERE ii.InvoiceId = @InvoiceId GROUP BY ii.[InvoiceId]) sp ON i.Id = sp.InvoiceId
			LEFT JOIN (SELECT SUM(ips.[PayAmount]) AS TotalPayments, ips.[InvoiceId] FROM InvoicePayments ips WHERE ips.InvoiceId = @InvoiceId GROUP BY ips.[InvoiceId]) pays ON i.Id = pays.InvoiceId
			WHERE i.Id = @InvoiceId);
	RETURN @Result
END
GO