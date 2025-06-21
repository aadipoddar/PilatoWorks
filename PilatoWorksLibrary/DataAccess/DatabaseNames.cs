namespace PilatoWorksLibrary.DataAccess;

public static class TableNames
{
	public static string Person => "Person";
	public static string Session => "Session";
	public static string SessionType => "SessionType";
	public static string Trainer => "Trainer";
	public static string User => "User";
	public static string Slot => "Slot";
	public static string Subscription => "Subscription";
	public static string PaymentModes => "PaymentMode";
	public static string SubscriptionPaymentDetails => "SubscriptionPaymentDetails";
}

public static class StoredProcedureNames
{
	public static string DeleteSessionById => "Delete_Session_By_Id";

	public static string LoadTableData => "Load_TableData";
	public static string LoadTableDataById => "Load_TableData_By_Id";
	public static string LoadTableDataByStatus => "Load_TableData_By_Status";

	public static string LoadUserByPassword => "Load_User_By_Password";
	public static string LoadPersonByNumber => "Load_Person_By_Number";
	public static string LoadPersonByNameNumber => "Load_Person_By_Name_Number";

	public static string LoadSubscriptionByPerson => "Load_Subscription_By_Person";
	public static string LoadValidSubscriptionByPerson => "Load_Valid_Subscription_By_Person";
	public static string LoadValidSubscriptionByDate => "Load_Valid_Subscription_By_Date";
	public static string LoadSubscriptionPaymentDetailsBySubscriptionId => "Load_SubscriptionPaymentDetails_By_SubscriptionId";
	public static string LoadSubscriptionByDateRange => "Load_Subscription_By_Date_Range";

	public static string LoadSessionDetailsByDateSlot => "Load_SessionDetails_By_Date_Slot";
	public static string LoadSessionDetailsByDateTrainer => "Load_SessionDetails_By_Date_Trainer";
	public static string LoadSessionDetailsByPersonId => "Load_SessionDetails_By_PersonId";
	public static string LoadSessionDetailsByDateRange => "Load_SessionDetails_By_Date_Range";

	public static string InsertSlot => "Insert_Slot";
	public static string InsertUser => "Insert_User";
	public static string InsertTrainer => "Insert_Trainer";

	public static string InsertPerson => "Insert_Person";
	public static string InsertSession => "Insert_Session";

	public static string InsertSubscription => "Insert_Subscription";
	public static string InsertSubscriptionPaymentDetails => "Insert_Subscription_PaymentDetails";
}

public static class ViewNames
{
	public static string SubscriptionDetails => "Subscription_Details";
	public static string ValidSubscriptionsDetails => "Valid_Subscription_Details";
	public static string SessionDetails => "Session_Details";
}