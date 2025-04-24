namespace PilatoWorksLibrary.Models;

public class SubscriptionModel
{
	public int Id { get; set; }
	public int PersonId { get; set; }
	public DateOnly ValidFrom { get; set; }
	public DateOnly ValidTo { get; set; }
	public int SessionTypeId { get; set; }
	public int NoSessions { get; set; }
	public int Booking { get; set; }
	public bool Status { get; set; }
}

public class ValidSubscriptionModel
{
	public int Id { get; set; }
	public int PersonId { get; set; }
	public string PersonName { get; set; }
	public string PersonNumber { get; set; }
	public DateOnly ValidFrom { get; set; }
	public DateOnly ValidTo { get; set; }
	public int SessionTypeId { get; set; }
	public int NoSessions { get; set; }
	public int Booking { get; set; }
	public bool Status { get; set; }
}