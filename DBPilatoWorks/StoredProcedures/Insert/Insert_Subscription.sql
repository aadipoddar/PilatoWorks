CREATE PROCEDURE [dbo].[Insert_Subscription]
	@Id INT,
	@PersonId INT,
	@ValidFrom DATE,
	@ValidTo DATE,
	@SessionTypeId INT,
	@NoSessions INT,
	@Amount INT,
	@PaymentModeId INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Subscription]
			([PersonId], [ValidFrom], [ValidTo], [SessionTypeId], [NoSessions], [Amount], [PaymentModeId], [Status])
		VALUES
			(@PersonId, @ValidFrom, @ValidTo, @SessionTypeId, @NoSessions, @Amount, @PaymentModeId, @Status);
	END

	ELSE
	BEGIN
	UPDATE [dbo].[Subscription]
		SET
			[PersonId] = @PersonId,
			[ValidFrom] = @ValidFrom,
			[ValidTo] = @ValidTo,
			[SessionTypeId] = @SessionTypeId,
			[NoSessions] = @NoSessions,
			[Amount] = @Amount,
			[PaymentModeId] = @PaymentModeId,
			[Status] = @Status
		WHERE
			Id = @Id;
	END
END