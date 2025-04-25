CREATE VIEW [dbo].[Subscription_Details]
	AS
SELECT 
    p.Id AS PersonId,
    p.Name AS PersonName,
    p.Number AS PersonNumber,
    s.Id AS SubscriptionId,
    s.ValidFrom AS SubscriptionValidFrom,
    s.ValidTo AS SubscriptionValidTo,
    s.NoSessions AS NoOfSessions,
    st.Id AS SessionTypeId,
    st.Name AS SessionType,
    s.Booking AS BookingAmount,
    s.NoSessions - ISNULL((
        SELECT COUNT(*)
        FROM dbo.[Session] AS sess
        WHERE sess.SubscriptionId = s.Id
          AND sess.Confirmed = 1
    ), 0) AS RemainingSessions,
    ISNULL(SUM(spd.Amount), 0) AS TotalPaid,
    (s.Booking - ISNULL(SUM(spd.Amount), 0)) AS Dues,
    s.UserId,
    u.[Name] AS UserName,
    s.SubscriptionDate
FROM 
    [dbo].[Subscription] s
INNER JOIN 
    [dbo].[Person] p ON s.PersonId = p.Id
LEFT JOIN 
    [dbo].[SubscriptionPaymentDetails] spd ON s.Id = spd.SubscriptionId AND spd.Status = 1
LEFT JOIN
    [dbo].[SessionType] st ON s.SessionTypeId = st.Id
LEFT JOIN
    [dbo].[User] u ON s.UserId = u.Id
WHERE
    s.Status = 1
GROUP BY 
    p.Id, p.Name, p.Number, s.Id, s.PersonId, s.ValidFrom, s.ValidTo, s.NoSessions, st.Id, st.Name, s.Booking, s.UserId, u.[Name], s.SubscriptionDate