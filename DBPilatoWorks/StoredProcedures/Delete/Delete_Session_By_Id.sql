CREATE PROCEDURE [dbo].[Delete_Session_By_Id]
	@SessionId INT
AS
BEGIN
	DELETE FROM [dbo].[Session]
	WHERE [Id] = @SessionId;
END