namespace PilatoWorksLibrary.Models;

public class SessionModel
{
	public int Id { get; set; }
	public DateOnly SessionDate { get; set; }
	public int SlotId { get; set; }
	public int SubscriptionId { get; set; }
	public int Trainer1Id { get; set; }
	public int? Trainer2Id { get; set; }
	public bool Confirmed { get; set; }
}

public class SessionDetailsModel
{
	public int Id { get; set; }
	public DateOnly SessionDate { get; set; }
	public int SlotId { get; set; }
	public int SlotHour { get; set; }
	public int SubscriptionId { get; set; }
	public int PersonId { get; set; }
	public string PersonName { get; set; }
	public string PersonNumber { get; set; }
	public int SessionTypeId { get; set; }
	public string SessionTypeName { get; set; }
	public int Trainer1Id { get; set; }
	public string Trainer1Name { get; set; }
	public int? Trainer2Id { get; set; }
	public string Trainer2Name { get; set; }
	public bool Confirmed { get; set; }
}