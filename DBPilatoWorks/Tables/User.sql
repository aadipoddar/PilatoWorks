CREATE TABLE [dbo].[User]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(20) NOT NULL, 
    [Password] INT NOT NULL UNIQUE, 
    [Session] BIT NOT NULL, 
    [Trainer] BIT NOT NULL, 
    [Admin] BIT NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1
)
