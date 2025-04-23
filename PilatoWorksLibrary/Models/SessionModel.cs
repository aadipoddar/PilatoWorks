namespace PilatoWorksLibrary.Models;

public class SessionModel
{
	public int Id { get; set; }
	public DateOnly SessionDate { get; set; }
	public int SlotId { get; set; }
	public int PersonId { get; set; }
	public int? Trainer1Id { get; set; }
	public int? Trainer2Id { get; set; }
	public bool Confirmed { get; set; }
}
