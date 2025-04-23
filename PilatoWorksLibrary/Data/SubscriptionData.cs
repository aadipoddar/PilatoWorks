namespace PilatoWorksLibrary.Data;

public static class SubscriptionData
{
	public static async Task InsertSubscription(SubscriptionModel subscriptionModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertSubscription, subscriptionModel);

	public static async Task<ValidSubscriptionModel> LoadValidSubscriptionByPerson(int PersonId) =>
			(await SqlDataAccess.LoadData<ValidSubscriptionModel, dynamic>(StoredProcedureNames.LoadValidSubscriptionByPerson, new { PersonId })).FirstOrDefault();

	public static async Task<List<ValidSubscriptionModel>> LoadValidSubscriptionByDate(DateOnly ForDate) =>
			await SqlDataAccess.LoadData<ValidSubscriptionModel, dynamic>(StoredProcedureNames.LoadValidSubscriptionByDate, new { ForDate });
}
