CREATE PROCEDURE [dbo].[Load_SessionDetails_By_Date_Slot]
	@SessionDate DATE,
	@SlotId INT
AS
BEGIN
	SELECT
	*
	FROM Session_Details
	WHERE SessionDate = @SessionDate
	AND SlotId = @SlotId
END