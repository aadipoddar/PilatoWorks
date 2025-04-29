CREATE PROCEDURE [dbo].[Load_Subscription_By_Date_Range]
	@StartDate DATE,
	@EndDate DATE
AS
BEGIN
	SELECT *
	FROM Subscription_Details
	WHERE SubscriptionDate BETWEEN @StartDate AND @EndDate;
END