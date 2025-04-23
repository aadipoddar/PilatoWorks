namespace PilatoWorksLibrary.Data;

public static class SessionData
{
	public static async Task InsertSession(SessionModel sessionModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertSession, sessionModel);
}
