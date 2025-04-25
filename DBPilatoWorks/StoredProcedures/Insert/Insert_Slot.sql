CREATE PROCEDURE [dbo].[Insert_Slot]
	@Id INT,
	@Hour INT,
	@Max INT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Slot] ([Hour], [Max])
		VALUES (@Hour, @Max)
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Slot]
		SET [Hour] = @Hour,
			[Max] = @Max
		WHERE [Id] = @Id
	END
END