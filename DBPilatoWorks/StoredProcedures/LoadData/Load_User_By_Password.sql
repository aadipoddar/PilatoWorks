CREATE PROCEDURE [dbo].[Load_User_By_Password]
	@Password smallint
AS
BEGIN
	SELECT * FROM [User] WHERE [Password] = @Password
END