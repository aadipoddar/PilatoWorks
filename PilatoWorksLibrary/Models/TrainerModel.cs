namespace PilatoWorksLibrary.Models;

public class TrainerModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Number { get; set; }
	public string Address { get; set; }
	public int Salary { get; set; }
	public decimal CommissionPercent { get; set; }
	public bool Status { get; set; }
}
