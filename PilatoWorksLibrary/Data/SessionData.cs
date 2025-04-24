namespace PilatoWorksLibrary.Data;

public static class SessionData
{
	public static async Task InsertSession(SessionModel sessionModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertSession, sessionModel);

	public static async Task<List<SessionDetailsModel>> LoadSessionDetailsByDateSlot(DateOnly SessionDate, int SlotId) =>
			await SqlDataAccess.LoadData<SessionDetailsModel, dynamic>(StoredProcedureNames.LoadSessionDetailsByDateSlot, new { SessionDate, SlotId });

	public static async Task<List<SessionDetailsModel>> LoadSessionDetailsByDateTrainer(DateOnly BeginDate, DateOnly EndDate, int TrainerId) =>
			await SqlDataAccess.LoadData<SessionDetailsModel, dynamic>(StoredProcedureNames.LoadSessionDetailsByDateTrainer, new { BeginDate, EndDate, TrainerId });
}
