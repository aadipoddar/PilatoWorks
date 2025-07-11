﻿namespace PilatoWorksLibrary.Data;

public static class PersonData
{
	public static async Task InsertPerson(PersonModel personModel) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertPerson, personModel);

	public static async Task<PersonModel> LoadPersonByNumber(string PersonNumber) =>
		(await SqlDataAccess.LoadData<PersonModel, dynamic>(StoredProcedureNames.LoadPersonByNumber, new { PersonNumber })).FirstOrDefault();
}
