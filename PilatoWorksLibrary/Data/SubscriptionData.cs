namespace PilatoWorksLibrary.Data;

public static class SubscriptionData
{
	public static async Task<int> InsertSubscription(SubscriptionModel subscriptionModel) =>
			(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertSubscription, subscriptionModel)).FirstOrDefault();

	public static async Task<SubscriptionDetailsModel> LoadValidSubscriptionByPerson(int PersonId) =>
			(await SqlDataAccess.LoadData<SubscriptionDetailsModel, dynamic>(StoredProcedureNames.LoadValidSubscriptionByPerson, new { PersonId })).FirstOrDefault();

	public static async Task<List<SubscriptionDetailsModel>> LoadValidSubscriptionByDate(DateOnly ForDate) =>
			await SqlDataAccess.LoadData<SubscriptionDetailsModel, dynamic>(StoredProcedureNames.LoadValidSubscriptionByDate, new { ForDate });

	public static async Task InsertSubscriptionPaymentDetails(SubscriptionPaymentDetailsModel subscriptionPaymentDetailsModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertSubscriptionPaymentDetails, subscriptionPaymentDetailsModel);

	public static async Task<List<SubscriptionPaymentDetailsModel>> LoadSubscriptionPaymentDetailsBySubscriptionId(int SubscriptionId) =>
		await SqlDataAccess.LoadData<SubscriptionPaymentDetailsModel, dynamic>(StoredProcedureNames.LoadSubscriptionPaymentDetailsBySubscriptionId, new { SubscriptionId });

	public static async Task<List<SubscriptionDetailsModel>> LoadSubscriptionByPerson(int PersonId) =>
		await SqlDataAccess.LoadData<SubscriptionDetailsModel, dynamic>(StoredProcedureNames.LoadSubscriptionByPerson, new { PersonId });

	public static async Task<List<SubscriptionDetailsModel>> LoadSubscriptionByDateRange(DateOnly StartDate, DateOnly EndDate) =>
		await SqlDataAccess.LoadData<SubscriptionDetailsModel, dynamic>(StoredProcedureNames.LoadSubscriptionByDateRange, new { StartDate, EndDate });
}
