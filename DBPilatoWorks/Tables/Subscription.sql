CREATE TABLE [dbo].[Subscription]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PersonId] INT NOT NULL, 
    [ValidFrom] DATE NOT NULL, 
    [ValidTo] DATE NOT NULL, 
    [SessionTypeId] INT NOT NULL, 
    [NoSessions] INT NOT NULL, 
    [Booking] INT NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Subscription_ToPerson] FOREIGN KEY (PersonId) REFERENCES [Person](Id), 
    CONSTRAINT [FK_Subscription_ToSessionType] FOREIGN KEY (SessionTypeId) REFERENCES [SessionType](Id)
)
