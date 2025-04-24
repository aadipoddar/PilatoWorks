CREATE PROCEDURE [dbo].[Load_SessionDetails_By_Date_Trainer]
	@BeginDate DATE,
	@EndDate DATE,
	@TrainerId INT
AS
BEGIN
	SELECT
	*
	FROM Session_Details sd
	WHERE sd.SessionDate BETWEEN @BeginDate AND @EndDate
		AND (sd.Trainer1Id = @TrainerId OR sd.Trainer2Id = @TrainerId)
		AND sd.Confirmed = 1
	ORDER BY sd.SessionDate, sd.SlotHour
END