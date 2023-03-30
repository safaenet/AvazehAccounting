CREATE FUNCTION [dbo].[CalculateDiscountedInvoiceSum](@disType tinyint, @disVal decimal, @amountVal decimal)
                            RETURNS DECIMAL
                            AS
                            BEGIN
                            IF @disType IS NULL SET @disType = 0
							IF @disVal IS NULL SET @disVal = 0
							IF @amountVal IS NULL SET @amountVal = 0
                            RETURN  CASE
                            		WHEN @disType = 0 THEN @amountVal - (@disVal / 100 * @amountVal)
                            		WHEN @disType = 1 THEN @amountVal - @disVal
                            		END
                            END
GO