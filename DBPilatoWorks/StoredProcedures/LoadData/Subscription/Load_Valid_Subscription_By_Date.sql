CREATE PROCEDURE [dbo].[Load_Valid_Subscription_By_Date]
	@ForDate DATE
AS
BEGIN
	SELECT *
	FROM [dbo].[Valid_Subscriptions] AS s
	WHERE s.[ValidFrom] <= @ForDate
		AND s.[ValidTo] >= @ForDate
		AND s.[NoSessions] > (
			SELECT COUNT(*)
			FROM dbo.[Session] AS sess
			WHERE sess.SessionDate BETWEEN s.ValidFrom AND s.ValidTo
				AND sess.PersonId = s.PersonId
				AND sess.Confirmed = 1
		);
END