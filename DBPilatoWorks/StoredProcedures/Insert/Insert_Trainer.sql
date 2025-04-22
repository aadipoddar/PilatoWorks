CREATE PROCEDURE [dbo].[Insert_Trainer]
	@Id INT,
	@Name VARCHAR(100),
	@Number VARCHAR(10),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Trainer] ([Name], [Number], [Status])
		VALUES (@Name, @Number, @Status);
		
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Trainer]
		SET 
			[Name] = @Name,
			[Number] = @Number,
			[Status] = @Status
		WHERE [Id] = @Id;
		
	END
END