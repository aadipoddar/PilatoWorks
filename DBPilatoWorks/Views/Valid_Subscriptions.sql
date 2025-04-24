CREATE VIEW [dbo].[Valid_Subscriptions]
	AS
	SELECT
		s.[Id],
		s.[PersonId],
		p.[Name] AS PersonName,
		p.[Number] AS PersonNumber,
		s.[ValidFrom],
		s.[ValidTo],
		s.[SessionTypeId],
		s.[NoSessions],
		s.[Booking],
		s.[Status]
	FROM [dbo].[Subscription] AS s
	INNER JOIN
		[dbo].[Person] AS p ON s.PersonId = p.Id
	WHERE s.[Status] = 1
		AND s.ValidFrom <= GETDATE()
		AND s.ValidTo >= GETDATE()