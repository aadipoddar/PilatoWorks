CREATE PROCEDURE [dbo].[Load_SessionDetails_By_PersonId]
	@PersonId INT
AS
BEGIN
	SELECT
		sd.*
	FROM Session_Details sd
	WHERE sd.PersonId = @PersonId
	ORDER BY sd.SessionDate DESC
END