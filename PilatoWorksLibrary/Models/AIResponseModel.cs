namespace PilatoWorksLibrary.Models;

public class SessionResponseModel
{
	public string time { get; set; } = "";
	public string date { get; set; } = "";
	public string clientName { get; set; } = "";
}

public class ClientNameMatchResult
{
	public int matchedId { get; set; } = 0;
	public int confidence { get; set; } = 0;
	public bool exactMatch { get; set; } = false;
}