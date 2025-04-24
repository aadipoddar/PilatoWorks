CREATE PROCEDURE [dbo].[Load_Valid_Subscription_By_Date]
	@ForDate DATE
AS
BEGIN
	SELECT
		[s].[PersonId],
		[s].[PersonName],
		[s].[PersonNumber],
		[s].[SubscriptionId],
		[s].[SubscriptionValidFrom],
		[s].[SubscriptionValidTo],
		[s].[NoOfSessions],
		[s].[SessionTypeId],
		[s].[SessionType],
		[s].[BookingAmount],
		[s].[RemainingSessions],
		[s].[TotalPaid],
		[s].[Dues]
	FROM [dbo].[Subscription_Details] AS s
	INNER JOIN
		[dbo].[Person] AS p ON s.PersonId = p.Id
	WHERE s.[SubscriptionValidFrom] <= @ForDate
		AND s.[SubscriptionValidTo] >= @ForDate
		AND s.[RemainingSessions] > 0
END