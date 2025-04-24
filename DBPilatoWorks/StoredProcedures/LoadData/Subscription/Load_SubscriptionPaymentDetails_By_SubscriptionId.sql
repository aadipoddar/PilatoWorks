CREATE PROCEDURE [dbo].[Load_SubscriptionPaymentDetails_By_SubscriptionId]
	@SubscriptionId INT
AS
BEGIN
	SELECT
		*
	FROM
		dbo.SubscriptionPaymentDetails AS sp
	WHERE
		sp.SubscriptionId = @SubscriptionId
		AND sp.Status = 1
END