INSERT INTO dbo.Products(ProductName, BuyPrice, SellPrice, Barcode, Descriptions, DateCreated, TimeCreated)
SELECT AdventureWorksLT2017.SalesLT.Product.Name, 5000, 8000,
AdventureWorksLT2017.SalesLT.Product.ProductNumber, 
AdventureWorksLT2017.SalesLT.Product.ThumbnailPhotoFileName, '1400/01/01', '23:23:23'
FROM AdventureWorksLT2017.SalesLT.Product