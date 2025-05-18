CREATE PROCEDURE [dbo].[Load_Person_By_Name]
	@PersonName VARCHAR(50)
AS
BEGIN
	SELECT *
	FROM dbo.Person
	WHERE Name LIKE '%' + @PersonName + '%'
END