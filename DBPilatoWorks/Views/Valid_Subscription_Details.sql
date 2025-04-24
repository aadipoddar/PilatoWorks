CREATE VIEW [dbo].[Valid_Subscription_Details]
	AS
	SELECT
		[PersonId],
		[PersonName],
		[PersonNumber],
		[SubscriptionId],
		[SubscriptionValidFrom],
		[SubscriptionValidTo],
		[NoOfSessions],
		[SessionTypeId],
		[SessionType],
		[BookingAmount],
		[RemainingSessions],
		[TotalPaid],
		[Dues]
	FROM [dbo].[Subscription_Details] AS s
	WHERE s.SubscriptionValidFrom <= GETDATE()
		AND s.SubscriptionValidTo >= GETDATE()
		AND s.RemainingSessions > 0