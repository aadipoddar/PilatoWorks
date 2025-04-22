namespace PilatoWorksLibrary.Models;

public class UserModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Password { get; set; }
	public bool Session { get; set; }
	public bool Trainer { get; set; }
	public bool Admin { get; set; }
	public bool Status { get; set; }
}
