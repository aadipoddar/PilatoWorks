CREATE PROCEDURE [dbo].[Insert_Session]
	@Id INT,
	@SessionDate DATE,
	@SlotId INT,
	@PersonId INT,
	@Trainer1Id INT,
	@Trainer2Id INT,
	@Confirmed BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Session] ([SessionDate], [SlotId], [PersonId], [Trainer1Id], [Trainer2Id], [Confirmed])
		VALUES (@SessionDate, @SlotId, @PersonId, @Trainer1Id, @Trainer2Id, @Confirmed);
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Session]
		SET 
			[SessionDate] = @SessionDate,
			[SlotId] = @SlotId,
			[PersonId] = @PersonId,
			[Trainer1Id] = @Trainer1Id,
			[Trainer2Id] = @Trainer2Id,
			[Confirmed] = @Confirmed
		WHERE [Id] = @Id;
	END

END