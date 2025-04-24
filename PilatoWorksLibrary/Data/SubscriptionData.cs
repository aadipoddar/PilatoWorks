namespace PilatoWorksLibrary.Data;

public static class SubscriptionData
{
	public static async Task<int> InsertSubscription(SubscriptionModel subscriptionModel) =>
			(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertSubscription, subscriptionModel)).FirstOrDefault();

	public static async Task<ValidSubscriptionModel> LoadValidSubscriptionByPerson(int PersonId) =>
			(await SqlDataAccess.LoadData<ValidSubscriptionModel, dynamic>(StoredProcedureNames.LoadValidSubscriptionByPerson, new { PersonId })).FirstOrDefault();

	public static async Task<List<ValidSubscriptionModel>> LoadValidSubscriptionByDate(DateOnly ForDate) =>
			await SqlDataAccess.LoadData<ValidSubscriptionModel, dynamic>(StoredProcedureNames.LoadValidSubscriptionByDate, new { ForDate });

	public static async Task InsertSubscriptionPaymentDetails(SubscriptionPaymentDetailsModel subscriptionPaymentDetailsModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertSubscriptionPaymentDetails, subscriptionPaymentDetailsModel);

	public static async Task<List<SubscriptionPaymentDetailsModel>> LoadSubscriptionPaymentDetailsBySubscriptionId(int SubscriptionId) =>
		await SqlDataAccess.LoadData<SubscriptionPaymentDetailsModel, dynamic>(StoredProcedureNames.LoadSubscriptionPaymentDetailsBySubscriptionId, new { SubscriptionId });
}
