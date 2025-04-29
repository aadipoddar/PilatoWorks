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
	public int UserId { get; set; }
	public bool Status { get; set; }
	public DateTime SubscriptionDate { get; set; }
}

public class SubscriptionDetailsModel
{
	public int PersonId { get; set; }
	public string PersonName { get; set; }
	public string PersonNumber { get; set; }
	public int SubscriptionId { get; set; }
	public DateOnly SubscriptionValidFrom { get; set; }
	public DateOnly SubscriptionValidTo { get; set; }
	public int NoOfSessions { get; set; }
	public int SessionTypeId { get; set; }
	public string SessionType { get; set; }
	public int BookingAmount { get; set; }
	public int RemainingSessions { get; set; }
	public int TotalPaid { get; set; }
	public int Dues { get; set; }
	public int UserId { get; set; }
	public string UserName { get; set; }
	public DateTime SubscriptionDate { get; set; }
}