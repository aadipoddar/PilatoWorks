CREATE PROCEDURE [dbo].[Insert_Trainer]
	@Id INT,
	@Name VARCHAR(100),
	@Number VARCHAR(10),
	@Address VARCHAR(250),
	@Salary INT,
	@CommissionPercent DECIMAL(5, 2),
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Trainer] ([Name], [Number], [Address], [Salary], [CommissionPercent], [Status])
		VALUES (@Name, @Number, @Address, @Salary, @CommissionPercent, @Status);
		
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Trainer]
		SET 
			[Name] = @Name,
			[Number] = @Number,
			[Address] = @Address,
			[Salary] = @Salary,
			[CommissionPercent] = @CommissionPercent,
			[Status] = @Status
		WHERE [Id] = @Id;
		
	END
END