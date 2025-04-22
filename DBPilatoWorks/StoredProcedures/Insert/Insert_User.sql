CREATE PROCEDURE [dbo].[Insert_User]
	@Id INT,
	@Name VARCHAR(20),
	@Password INT,
	@Session BIT,
	@Trainer BIT,
	@Admin BIT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[User] ([Name], [Password], [Session], [Trainer], [Admin], [Status])
		VALUES (@Name, @Password, @Session, @Trainer, @Admin, @Status);
		
	END

	ELSE
	BEGIN
		UPDATE [dbo].[User]
		SET 
			[Name] = @Name,
			[Password] = @Password,
			[Session] = @Session,
			[Trainer] = @Trainer,
			[Admin] = @Admin,
			[Status] = @Status
		WHERE [Id] = @Id;
		
	END
END