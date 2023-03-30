CREATE FUNCTION [dbo].[CalculateInvoiceSellSum]
(
	@InvoiceId INT
)
RETURNS DECIMAL
AS
BEGIN
	DECLARE @Result DECIMAL = (SELECT SUM(ii.[SellSum]) FROM InvoiceItems ii WHERE ii.InvoiceId = @InvoiceId GROUP BY ii.[InvoiceId]);
	RETURN @Result
END
GO