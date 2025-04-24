CREATE TABLE [dbo].[Trainer]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(100) NOT NULL, 
    [Number] VARCHAR(10) NOT NULL UNIQUE, 
    [Address] VARCHAR(250) NOT NULL , 
    [Salary] INT NOT NULL ,
    [CommissionPercent] DECIMAL(5, 2) NOT NULL , 
    [Status] BIT NOT NULL DEFAULT 1
)
