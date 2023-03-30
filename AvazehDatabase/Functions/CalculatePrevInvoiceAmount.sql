CREATE FUNCTION [dbo].[CalculatePrevInvoiceAmount] (@InvoiceId INT)
RETURNS DECIMAL
AS
BEGIN
	DECLARE @PrevId INT = (SELECT i.PrevInvoiceId FROM dbo.Invoices i WHERE i.Id = @InvoiceId AND i.LifeStatus IN (0, 2))
	IF (@PrevId IS NULL) RETURN 0
	DECLARE @Amount DECIMAL = [dbo].[CalculateInvoiceBalance](@PrevId);
	RETURN (@Amount + [dbo].[CalculatePrevInvoiceAmount](@PrevId))
END
GO