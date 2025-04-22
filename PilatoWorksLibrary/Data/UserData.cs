using PilatoWorksLibrary.Models;

namespace PilatoWorksLibrary.Data;

public static class UserData
{
	public static async Task InsertUser(UserModel userModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertUser, userModel);

	public static async Task<UserModel> LoadUserByPassword(string Password) =>
			(await SqlDataAccess.LoadData<UserModel, dynamic>(StoredProcedureNames.LoadUserByPassword, new { Password })).FirstOrDefault();
}
