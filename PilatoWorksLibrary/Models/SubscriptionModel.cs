namespace PilatoWorksLibrary.Models;

public class SubscriptionModel
{
	public int Id { get; set; }
	public int PersonId { get; set; }
	public DateOnly ValidFrom { get; set; }
	public DateOnly ValidTo { get; set; }
	public int SessionTypeId { get; set; }
	public int NoSessions { get; set; }
	public int Amount { get; set; }
	public int PaymentModeId { get; set; }
	public bool Status { get; set; }
}
