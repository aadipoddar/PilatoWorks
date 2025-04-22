namespace PilatoWorksLibrary.DataAccess;

public static class TableNames
{
	public static string Person => "Person";
	public static string Session => "Session";
	public static string SessionType => "SessionType";
	public static string Trainer => "Trainer";
	public static string User => "User";
}

public static class StoredProcedureNames
{
	public static string LoadTableData => "Load_TableData";
	public static string LoadTableDataById => "Load_TableData_By_Id";
	public static string LoadTableDataByStatus => "Load_TableData_By_Status";

	public static string LoadUserByPassword => "Load_User_By_Password";

	public static string InsertPerson => "Insert_Person";
	public static string InsertSession => "Insert_Session";
	public static string InsertTrainer => "Insert_Trainer";
	public static string InsertUser => "Insert_User";
}
