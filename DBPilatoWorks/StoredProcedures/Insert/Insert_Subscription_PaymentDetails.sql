CREATE PROCEDURE [dbo].[Insert_Subscription_PaymentDetails]
	@Id INT,
	@SubscriptionId INT,
	@Amount INT,
	@PaymentModeId INT,
	@PaymentDate DATETIME,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [SubscriptionPaymentDetails] ([SubscriptionId], [Amount], [PaymentModeId], [Status])
		VALUES (@SubscriptionId, @Amount, @PaymentModeId, @Status);
	END

	ELSE
	BEGIN
		UPDATE [SubscriptionPaymentDetails]
		SET [SubscriptionId] = @SubscriptionId,
			[Amount] = @Amount,
			[PaymentModeId] = @PaymentModeId,
			[Status] = @Status
		WHERE [Id] = @Id;
	END
END