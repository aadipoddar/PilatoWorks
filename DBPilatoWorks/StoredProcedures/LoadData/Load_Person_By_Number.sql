CREATE PROCEDURE [dbo].[Load_Person_By_Number]
	@PersonNumber VARCHAR(10)
AS
BEGIN
	SELECT *
	FROM dbo.Person
	WHERE Number = @PersonNumber
END