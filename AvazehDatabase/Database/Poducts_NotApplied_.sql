CREATE TABLE [dbo].[Poducts]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1000, 1), 
    [PName] NVARCHAR(100) NOT NULL, 
	[UnitType] INT NOT NULL,
	[MainUnit] INT NOT NULL,
	[NumberInMainUnit] INT NOT NULL,
    [Barcode] VARCHAR(15) NULL, 
    [Description_] NTEXT NULL
)
