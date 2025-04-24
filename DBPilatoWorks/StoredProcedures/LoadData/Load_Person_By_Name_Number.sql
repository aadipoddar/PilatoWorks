CREATE PROCEDURE [dbo].[Load_Person_By_Name_Number]
	@PersonName VARCHAR(50),
	@PersonNumber VARCHAR(10)
AS
BEGIN
	SELECT *
	FROM dbo.Person
	WHERE Number LIKE '%' + @PersonNumber + '%'
	AND Name LIKE '%' + @PersonName + '%'
END