CREATE VIEW dbo.Session_Details AS
SELECT
    [s].[Id],
    [s].[SessionDate],
    [s].[SlotId],
    [sl].[Hour] AS SlotHour,
    [s].[SubscriptionId],
    [p].[Id] AS PersonId,
    [p].[Name] AS PersonName,
    [p].[Number] AS PersonNumber,
    [sb].[SessionTypeId],
    [st].[Name] AS SessionTypeName,
    [s].[Trainer1Id],
    [t1].[Name] AS Trainer1Name,
    [s].[Trainer2Id],
    [t2].[Name] AS Trainer2Name,
    [s].[Confirmed]
FROM dbo.[Session] s
INNER JOIN
    dbo.[Slot] sl ON sl.Id = s.SlotId
LEFT JOIN 
    dbo.[Trainer] t1 ON t1.Id = s.Trainer1Id
LEFT JOIN
    dbo.[Trainer] t2 ON t2.Id = s.Trainer2Id
INNER JOIN
    dbo.[Subscription] sb ON sb.Id = s.SubscriptionId
INNER JOIN
    dbo.[SessionType] st ON st.Id = sb.SessionTypeId
INNER JOIN
    dbo.[Person] p ON p.Id = sb.PersonId