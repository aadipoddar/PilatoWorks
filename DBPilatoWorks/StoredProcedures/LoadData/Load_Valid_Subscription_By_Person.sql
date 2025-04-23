CREATE PROCEDURE [dbo].[Load_Valid_Subscription_By_Person]
	@PersonId INT
AS
BEGIN
	SELECT *
	FROM [dbo].[Subscription] AS s
	WHERE s.[Status] = 1
		AND s.PersonId = @PersonId
		AND s.ValidFrom <= GETDATE()
		AND s.ValidTo > GETDATE()
END