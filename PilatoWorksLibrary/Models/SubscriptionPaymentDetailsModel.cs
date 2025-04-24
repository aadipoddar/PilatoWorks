namespace PilatoWorksLibrary.Models;

public class SubscriptionPaymentDetailsModel
{
	public int Id { get; set; }
	public int SubscriptionId { get; set; }
	public int Amount { get; set; }
	public int PaymentModeId { get; set; }
	public DateTime PaymentDate { get; set; }
	public bool Status { get; set; }
}

public class SubscriptionPaymentModeModel
{
	public int Id { get; set; }
	public int PaymentModeId { get; set; }
	public string PaymentModeName { get; set; }
	public int Amount { get; set; }
}