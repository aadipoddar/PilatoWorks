namespace PilatoWorksLibrary.Data;

public static class SubscriptionData
{
	public static async Task InsertSubscription(SubscriptionModel subscriptionModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertSubscription, subscriptionModel);

	public static async Task<SubscriptionModel> LoadValidSubscriptionByPerson(int PersonId) =>
			(await SqlDataAccess.LoadData<SubscriptionModel, dynamic>(StoredProcedureNames.LoadValidSubscriptionByPerson, new { PersonId })).FirstOrDefault();
}
