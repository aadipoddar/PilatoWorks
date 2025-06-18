using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PilatoWorks.Components.Pages.Panels;

internal static class AIProcessing
{
	internal static async Task<SessionResponseModel> SessionProcessing(string transcript)
	{
		SessionResponseModel extractedInfo = new();

		try
		{
			using var httpClient = new HttpClient();
			httpClient.BaseAddress = new Uri("https://api.aimlapi.com/v1/");
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Secrets.OpenAPI);

			string prompt = "Extract the following information from the provided text: " +
						"1. Time in 24-hour format " +
						"2. Date in dd/mm format " +
						"3. Client name " +
						"Return ONLY a JSON object with these keys: 'time', 'date', 'clientName'. " +
						transcript;

			var requestBody = JsonSerializer.Serialize(new
			{
				model = "google/gemma-3n-e4b-it",
				messages = new[]
				{
					new
					{
						role = "user",
						content = prompt
					}
				},
				temperature = 0.7,
				top_p = 0.7,
				frequency_penalty = 1,
				max_output_tokens = 512,
				top_k = 50
			});

			var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync("chat/completions", content);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();

			using JsonDocument outputAsJson = JsonDocument.Parse(responseContent);
			string messageContent = outputAsJson.RootElement
				.GetProperty("choices")[0]
				.GetProperty("message")
				.GetProperty("content")
				.GetString() ?? string.Empty;

			// Extract and parse the JSON from the response
			int jsonStart = messageContent.IndexOf('{');
			int jsonEnd = messageContent.LastIndexOf('}') + 1;

			if (jsonStart >= 0 && jsonEnd > jsonStart)
			{
				string jsonContent = messageContent[jsonStart..jsonEnd];
				extractedInfo = JsonSerializer.Deserialize<SessionResponseModel>(jsonContent);

				if (extractedInfo is not null)
				{
					extractedInfo.time = extractedInfo.time[..Math.Min(2, extractedInfo.time.Length)];
					if (string.IsNullOrEmpty(extractedInfo.time))
						extractedInfo.time = DateTime.Now.Hour.ToString();

					string[] dateComponents = extractedInfo.date.Split('/');
					if (dateComponents.Length == 2 &&
						int.TryParse(dateComponents[0], out int day) &&
						int.TryParse(dateComponents[1], out int month))
						extractedInfo.date = new DateOnly(DateTime.Now.Year, month, day).ToString();

					else
						extractedInfo.date = DateOnly.FromDateTime(DateTime.Now).ToString();

					if (DateTime.Parse(extractedInfo.date) < DateTime.Now.AddDays(-1))
						extractedInfo.date = DateOnly.FromDateTime(DateTime.Now).ToString();
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error processing transcript with Google Gemma API: {ex.Message}");
		}

		return extractedInfo;
	}

	/// <summary>
	/// Find the perfect client name match using AI-powered fuzzy matching
	/// </summary>
	/// <param name="extractedClientName">The client name extracted from user input</param>
	/// <param name="availableClients">List of available client names to match against</param>
	/// <returns>The best matching client information or null if no good match found</returns>
	internal static async Task<SubscriptionDetailsModel> FindBestClientMatch(
		string extractedClientName,
		List<SubscriptionDetailsModel> availableClients)
	{
		if (string.IsNullOrWhiteSpace(extractedClientName) || availableClients == null || availableClients.Count == 0)
			return null;

		try
		{
			// Create the HTTP client
			using var httpClient = new HttpClient();
			httpClient.BaseAddress = new Uri("https://api.aimlapi.com/v1/");
			httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", Secrets.OpenAPI);

			// Create a serializable list of names for the AI to compare
			var clientList = availableClients.Select(c =>
				new { id = c.SubscriptionId, name = c.PersonName }).ToList();

			// Create the message content for precise name matching
			string prompt = $@"I need to find the best matching client name from a list. 
The user mentioned: ""{extractedClientName}"" 

Here are the available client names: 
{JsonSerializer.Serialize(clientList)}

Find the most likely match for ""{extractedClientName}"" from the list. 
Consider nicknames, misspellings, and partial names. 
Only return results with high confidence.
Return ONLY a JSON object with these properties:
- matchedId: The subscription ID of the matched client
- confidence: A number between 0 and 100 indicating match confidence
- exactMatch: Boolean, true if it's an exact match

If there's no good match, set matchedId to 0, confidence to 0, and exactMatch to false.";

			// Create the request payload
			var requestBody = JsonSerializer.Serialize(new
			{
				model = "google/gemma-3n-e4b-it",
				messages = new[]
				{
					new
					{
						role = "user",
						content = prompt
					}
				},
				temperature = 0.3, // Lower temperature for more precise matching
				top_p = 0.9,
				frequency_penalty = 0,
				max_output_tokens = 256,
				top_k = 40
			});

			// Send the request
			var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync("chat/completions", content);
			response.EnsureSuccessStatusCode();

			// Parse the response
			var responseContent = await response.Content.ReadAsStringAsync();
			using JsonDocument outputAsJson = JsonDocument.Parse(responseContent);
			string messageContent = outputAsJson.RootElement
				.GetProperty("choices")[0]
				.GetProperty("message")
				.GetProperty("content")
				.GetString() ?? string.Empty;

			// Extract JSON from response
			int jsonStart = messageContent.IndexOf('{');
			int jsonEnd = messageContent.LastIndexOf('}') + 1;

			if (jsonStart >= 0 && jsonEnd > jsonStart)
			{
				string jsonContent = messageContent[jsonStart..jsonEnd];
				var matchResult = JsonSerializer.Deserialize<ClientNameMatchResult>(jsonContent);

				if (matchResult != null && matchResult.matchedId > 0 && matchResult.confidence >= 60)
				{
					// Return the matched client if confidence is sufficient
					return availableClients.FirstOrDefault(c => c.SubscriptionId == matchResult.matchedId);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error finding client match with AI: {ex.Message}");
		}

		return null;
	}
}