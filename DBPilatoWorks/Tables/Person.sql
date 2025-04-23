CREATE TABLE [dbo].[Person]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(100) NOT NULL, 
    [DateOfBirth] DATE NOT NULL, 
    [Gender] VARCHAR(10) NOT NULL, 
    [Number] VARCHAR(10) NOT NULL UNIQUE, 
    [Email] VARCHAR(100) NOT NULL, 
    [Address] VARCHAR(250) NOT NULL, 
    [EmerrgencyName] VARCHAR(100) NOT NULL, 
    [EmergencyRelationship] VARCHAR(100) NOT NULL, 
    [EmergencyNumber] VARCHAR(10) NOT NULL, 
    [ExistingMedicalCondition] VARCHAR(250) NOT NULL, 
    [CurrentlyOnMedication] VARCHAR(250) NOT NULL, 
    [SurgeriesTwoYears] VARCHAR(250) NOT NULL, 
    [FitnessGoals] VARCHAR(250) NOT NULL, 
    [CurrentFitnessLevel] VARCHAR(250) NOT NULL, 
    [OftenExercise] VARCHAR(250) NOT NULL, 
    [Diet] VARCHAR(250) NOT NULL
)
