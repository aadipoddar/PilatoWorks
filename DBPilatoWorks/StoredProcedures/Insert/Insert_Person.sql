CREATE PROCEDURE [dbo].[Insert_Person]
	@Id INT,
	@Name VARCHAR(100),
	@DateOfBirth DATE,
	@Gender VARCHAR(10),
	@Number VARCHAR(10),
	@Email VARCHAR(100),
	@Address VARCHAR(250),
	@EmergencyName VARCHAR(100),
	@EmergencyRelationship VARCHAR(100),
	@EmergencyNumber VARCHAR(10),
	@ExistingMedicalCondition VARCHAR(250),
	@CurrentlyOnMedication VARCHAR(250),
	@SurgeriesTwoYears VARCHAR(250),
	@FitnessGoals VARCHAR(250),
	@CurrentFitnessLevel VARCHAR(250),
	@OftenExercise VARCHAR(250),
	@Diet VARCHAR(250)
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Person]
		(
			[Name], 
			[DateOfBirth], 
			[Gender], 
			[Number], 
			[Email], 
			[Address], 
			[EmergencyName], 
			[EmergencyRelationship], 
			[EmergencyNumber], 
			[ExistingMedicalCondition], 
			[CurrentlyOnMedication], 
			[SurgeriesTwoYears], 
			[FitnessGoals], 
			[CurrentFitnessLevel],
			[OftenExercise],
			[Diet]
		)
		VALUES
		(
			@Name, 
			@DateOfBirth, 
			@Gender, 
			@Number, 
			@Email, 
			@Address, 
			@EmergencyName, 
			@EmergencyRelationship, 
			@EmergencyNumber, 
			@ExistingMedicalCondition, 
			@CurrentlyOnMedication, 
			@SurgeriesTwoYears, 
			@FitnessGoals, 
			@CurrentFitnessLevel,
			@OftenExercise,
			@Diet
		)
	END

	ELSE
	BEGIN
	UPDATE [dbo].[Person]
		SET
			[Name] = @Name, 
			[DateOfBirth] = @DateOfBirth, 
			[Gender] = @Gender, 
			[Number] = @Number, 
			[Email] = @Email, 
			[Address] = @Address, 
			[EmergencyName] = @EmergencyName, 
			[EmergencyRelationship] = @EmergencyRelationship, 
			[EmergencyNumber] = @EmergencyNumber, 
			[ExistingMedicalCondition] = @ExistingMedicalCondition, 
			[CurrentlyOnMedication] = @CurrentlyOnMedication, 
			[SurgeriesTwoYears] = @SurgeriesTwoYears, 
			[FitnessGoals] = @FitnessGoals, 
			[CurrentFitnessLevel] = @CurrentFitnessLevel,
			[OftenExercise] = @OftenExercise,
			[Diet] = @Diet
		WHERE [Id] = @Id
	END

END