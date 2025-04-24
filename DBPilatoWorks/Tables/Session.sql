CREATE TABLE [dbo].[Session]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SessionDate] DATE NOT NULL, 
    [SlotId] INT NOT NULL, 
    [SubscriptionId] INT NOT NULL,
    [Trainer1Id] INT NOT NULL, 
    [Trainer2Id] INT NULL, 
    [Confirmed] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_Session_ToSubscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [Subscription](Id), 
    CONSTRAINT [FK_Session_ToTrainer1] FOREIGN KEY (Trainer1Id) REFERENCES [Trainer](Id),
    CONSTRAINT [FK_Session_ToTrainer2] FOREIGN KEY (Trainer2Id) REFERENCES [Trainer](Id)
)
