CREATE PROCEDURE [dbo].[Load_Subscription_By_Person]
	@PersonId INT
AS
BEGIN
	SELECT *
	FROM [dbo].[Subscription_Details] AS s
	WHERE s.[PersonId] = @PersonId
	ORDER BY s.[SubscriptionId] DESC
END