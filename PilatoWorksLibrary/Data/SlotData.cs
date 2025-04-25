namespace PilatoWorksLibrary.Data;

public static class SlotData
{
	public static async Task InsertSlot(SlotModel slotModel) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertSlot, slotModel);
}
