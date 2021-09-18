SELECT i.Id, c.FirstName + ' ' + c.LastName FullName, i.DateCreated, i.DateUpdated FROM Invoices i LEFT JOIN Customers c ON i.CustomerId = c.Id


DECLARE @Expression TABLE (ID INT, Expression NCHAR(50))
INSERT @Expression SELECT Id, CountString FROM InvoiceItems


IF OBJECT_ID(N'tempdb..#Results') IS NOT NULL
BEGIN
    DROP TABLE #Results
END
Create table #Results (ID int,Value varchar(max))

Declare @SQL varchar(max) = ''
Select  @SQL = @SQL + concat(',(', ID, ',cast(', Expression, ' as nchar(50)))') From @Expression 
Select  @SQL = 'Insert Into #Results Select * From (' + Stuff(@SQL, 1, 1, 'values') + ') N(ID,Value)'
Exec(@SQL)


Select * From @Expression
Select * From #Results