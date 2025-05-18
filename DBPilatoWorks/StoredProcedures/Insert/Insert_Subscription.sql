CREATE PROCEDURE [dbo].[Insert_Subscription]
	@Id INT OUTPUT,
	@PersonId INT,
	@ValidFrom DATE,
	@ValidTo DATE,
	@SessionTypeId INT,
	@NoSessions INT,
	@Booking INT,
	@UserId INT,
	@Status BIT,
	@SubscriptionDate DATETIME
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Subscription]
			([PersonId], [ValidFrom], [ValidTo], [SessionTypeId], [NoSessions], [Booking], [UserId], [Status], [SubscriptionDate])
		VALUES
			(@PersonId, @ValidFrom, @ValidTo, @SessionTypeId, @NoSessions, @Booking, @UserId, @Status, @SubscriptionDate);
		SET @Id = SCOPE_IDENTITY();
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
			[Booking] = @Booking,
			[UserId] = @UserId,
			[Status] = @Status,
			[SubscriptionDate] = @SubscriptionDate
		WHERE
			Id = @Id;
	END

	SELECT @Id AS Id;
END