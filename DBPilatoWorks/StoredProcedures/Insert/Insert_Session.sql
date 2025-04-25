CREATE PROCEDURE [dbo].[Insert_Session]
	@Id INT,
	@SessionDate DATE,
	@SlotId INT,
	@SubscriptionId INT,
	@Trainer1Id INT,
	@Trainer2Id INT,
	@Confirmed BIT,
	@UserId INT,
	@CreatedDate DATETIME
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Session] ([SessionDate], [SlotId], [SubscriptionId], [Trainer1Id], [Trainer2Id], [Confirmed], [UserId])
		VALUES (@SessionDate, @SlotId, @SubscriptionId, @Trainer1Id, @Trainer2Id, @Confirmed, @UserId);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Session]
		SET 
			[SessionDate] = @SessionDate,
			[SlotId] = @SlotId,
			[SubscriptionId] = @SubscriptionId,
			[Trainer1Id] = @Trainer1Id,
			[Trainer2Id] = @Trainer2Id,
			[Confirmed] = @Confirmed,
			[UserId] = @UserId
		WHERE [Id] = @Id;
	END

END