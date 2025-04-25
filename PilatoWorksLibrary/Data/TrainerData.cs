namespace PilatoWorksLibrary.Data;

public static class TrainerData
{
	public static async Task InsertTrainer(TrainerModel trainerModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertTrainer, trainerModel);
}
