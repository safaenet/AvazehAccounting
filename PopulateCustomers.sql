INSERT INTO dbo.Customers(FirstName, LastName, CompanyName, EmailAddress)
SELECT AdventureWorksLT2017.SalesLT.Customer.FirstName, 
AdventureWorksLT2017.SalesLT.Customer.LastName, 
AdventureWorksLT2017.SalesLT.Customer.CompanyName, 
AdventureWorksLT2017.SalesLT.Customer.EmailAddress
FROM AdventureWorksLT2017.SalesLT.Customer