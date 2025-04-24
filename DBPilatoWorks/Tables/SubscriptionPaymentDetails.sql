CREATE TABLE [dbo].[SubscriptionPaymentDetails]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SubscriptionId] INT NOT NULL, 
    [Amount] INT NOT NULL, 
    [PaymentModeId] INT NOT NULL, 
    [PaymentDate] DATETIME NOT NULL DEFAULT (((getdate() AT TIME ZONE 'UTC') AT TIME ZONE 'India Standard Time')), 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_SubscriptionPaymentDetails_ToPaymentMode] FOREIGN KEY (PaymentModeId) REFERENCES [PaymentMode](Id), 
    CONSTRAINT [FK_SubscriptionPaymentDetails_ToSubscription] FOREIGN KEY (SubscriptionId) REFERENCES [Subscription](Id)
)
