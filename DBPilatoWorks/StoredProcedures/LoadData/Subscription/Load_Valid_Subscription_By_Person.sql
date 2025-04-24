CREATE PROCEDURE [dbo].[Load_Valid_Subscription_By_Person]
	@PersonId INT
AS
BEGIN
	SELECT *
	FROM [dbo].[Valid_Subscription_Details] AS s
	WHERE s.[PersonId] = @PersonId
END