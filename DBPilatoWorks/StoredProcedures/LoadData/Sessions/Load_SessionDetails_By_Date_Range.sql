CREATE PROCEDURE [dbo].[Load_SessionDetails_By_Date_Range]
	@BeginDate DATE,
	@EndDate DATE
AS
BEGIN
	SELECT	*
	FROM Session_Details
	WHERE SessionDate BETWEEN @BeginDate AND @EndDate
END