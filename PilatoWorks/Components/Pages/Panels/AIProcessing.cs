using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PilatoWorks.Components.Pages.Panels;

internal static class AIProcessing
{
	private static async Task<string> GetAIResponse(string prompt)
	{
		using var httpClient = new HttpClient();
		httpClient.BaseAddress = new Uri("https://api.aimlapi.com/v1/");
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Secrets.OpenAPI);

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

		if (!response.IsSuccessStatusCode)
			throw new HttpRequestException($"AI API request failed with status code {response.StatusCode}");

		string responseContent = await response.Content.ReadAsStringAsync();

		using JsonDocument outputAsJson = JsonDocument.Parse(responseContent);
		string messageContent = outputAsJson.RootElement
			.GetProperty("choices")[0]
			.GetProperty("message")
			.GetProperty("content")
			.GetString() ?? string.Empty;

		int jsonStart = messageContent.IndexOf('{');
		int jsonEnd = messageContent.LastIndexOf('}') + 1;

		if (jsonStart >= 0 && jsonEnd > jsonStart)
		{
			string jsonContent = messageContent[jsonStart..jsonEnd];
			return jsonContent;
		}

		throw new InvalidOperationException("AI response did not contain valid JSON");
	}

	internal static async Task<SessionResponseModel> ExtractSessionDetailsFromTranscript(string transcript)
	{
		if (string.IsNullOrEmpty(transcript))
			return new SessionResponseModel
			{
				time = DateTime.Now.Hour.ToString(),
				date = DateOnly.FromDateTime(DateTime.Now).ToString(),
				clientName = string.Empty
			};

		SessionResponseModel extractedInfo = new();

		try
		{
			string prompt = "Extract the following information from the provided text: " +
						"1. Time in 24-hour format " +
						"2. Date in dd/mm format " +
						"3. Client name " +
						"Return ONLY a JSON object with these keys: 'time', 'date', 'clientName'. " +
						transcript;

			string aiJSONResponse = await GetAIResponse(prompt);

			extractedInfo = JsonSerializer.Deserialize<SessionResponseModel>(aiJSONResponse);

			if (extractedInfo is null || string.IsNullOrEmpty(extractedInfo.time))
				return new SessionResponseModel
				{
					time = DateTime.Now.Hour.ToString(),
					date = DateOnly.FromDateTime(DateTime.Now).ToString(),
					clientName = string.Empty
				};

			extractedInfo.time = extractedInfo.time[..Math.Min(2, extractedInfo.time.Length)];
			if (string.IsNullOrEmpty(extractedInfo.time))
				extractedInfo.time = DateTime.Now.Hour.ToString();

			var slots = await CommonData.LoadTableData<SlotModel>(TableNames.Slot);
			var matchingSlot = slots.FirstOrDefault(s => s.Hour == int.Parse(extractedInfo.time));

			if (matchingSlot is not null)
				extractedInfo.time = matchingSlot.Hour.ToString();
			else
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
		catch (Exception ex)
		{
			Console.WriteLine($"Error processing transcript with AI: {ex.Message}");
		}

		return extractedInfo;
	}

	internal static async Task<SubscriptionDetailsModel> FindBestClientMatch(string extractedClientName, List<SubscriptionDetailsModel> availableClients)
	{
		if (string.IsNullOrWhiteSpace(extractedClientName) || availableClients is null || availableClients.Count == 0)
			return null;

		try
		{
			var clientList = availableClients.Select(c =>
				new { id = c.SubscriptionId, name = c.PersonName }).ToList();

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

			var aiJSONResponse = await GetAIResponse(prompt);

			var matchResult = JsonSerializer.Deserialize<ClientNameMatchResult>(aiJSONResponse);

			if (matchResult is not null && matchResult.matchedId > 0 && matchResult.confidence >= 60)
				return availableClients.FirstOrDefault(c => c.SubscriptionId == matchResult.matchedId);

			return new SubscriptionDetailsModel
			{
				SubscriptionId = 0,
				PersonName = string.Empty
			};
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error finding client match with AI: {ex.Message}");
		}

		return null;
	}
}