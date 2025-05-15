using Syncfusion.Blazor.Grids;

namespace PilatoWorksWeb.Components.Pages.Reports;

public partial class PersonReport
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	[Parameter]
	public string PhoneNumber { get; set; }

	private readonly string[] _pageSizes = ["5", "10", "15", "20"];

	private List<SessionDetailsModel> _sessionDetailsModels = [];
	private List<SubscriptionDetailsModel> _personSubscriptionModels = [];

	private SfGrid<SessionDetailsModel> _sfSessionGrid;
	private SfGrid<SubscriptionDetailsModel> _sfPersonSubscriptionGrid;

	private string _searchPhoneNumber;
	private string _clientName;
	private bool _hasSearched = false;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");

		if (!firstRender) return;

		_sessionDetailsModels = [];
		_personSubscriptionModels = [];

		if (!string.IsNullOrEmpty(PhoneNumber))
		{
			_searchPhoneNumber = PhoneNumber;
			await SearchSessionsByPhoneNumber();
		}
		else
		{
			var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
			if (Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("phoneNumber", out var phoneParam))
			{
				_searchPhoneNumber = phoneParam;
				await SearchSessionsByPhoneNumber();
			}
		}
	}

	private async Task<bool> ValidatePassword()
	{
		var userId = await JS.InvokeAsync<string>("getCookie", "UserId");
		var password = await JS.InvokeAsync<string>("getCookie", "Password");

		if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
			return false;

		var user = await CommonData.LoadTableDataById<UserModel>(TableNames.User, int.Parse(userId));
		if (user is null || !BCrypt.Net.BCrypt.EnhancedVerify(user.Password.ToString(), password))
			return false;

		return true;
	}

	private async Task SearchSessionsByPhoneNumber()
	{
		if (string.IsNullOrEmpty(_searchPhoneNumber)) return;

		var person = await PersonData.LoadPersonByNumber(_searchPhoneNumber);
		if (person is null)
		{
			await JS.InvokeVoidAsync("alert", "Person not found. Please check the phone number.");
			return;
		}

		_hasSearched = true;
		StateHasChanged();

		_sessionDetailsModels = await SessionData.LoadSessionDetailsByPersonId(person.Id);
		_personSubscriptionModels = await SubscriptionData.LoadSubscriptionByPerson(person.Id);

		await _sfSessionGrid?.Refresh();
		await _sfPersonSubscriptionGrid?.Refresh();

		if (_sessionDetailsModels?.Count > 0)
			_clientName = _sessionDetailsModels[0].PersonName;

		StateHasChanged();
	}

	private void OnBackClick() =>
		NavManager.NavigateTo("/Reports");

	private async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
	{
		if (args.Item.Id == "_sfPersonSubscriptionGrid_excelexport")
		{
			// Create summary items for report
			var summaryItems = new Dictionary<string, object>
			{
				{ "Total Sessions", _sessionDetailsModels?.Count ?? 0},
				{ "Active Subscriptions", _personSubscriptionModels?.Count ?? 0},
				{ "Remainig Sessions", CalculateRemainingSessionsTotal() },
				{ "Total Dues", CalculateDuesTotal() }
			};

			var hiddenColumns = new List<int>
			{
				0,
				3,
				7,
				13
			};

			// Use the generalized Excel exporter
			var stream = ExcelExportUtil.ExportToExcel(
				data: _personSubscriptionModels,
				reportTitle: "CLIENT SUBSCRIPTION REPORT",
				worksheetName: "Client Subscription Details",
				summaryItems: summaryItems);

			// Save the file with a descriptive name
			var fileName = $"Client_Subscription_Report.xlsx";
			await JS.InvokeVoidAsync("saveAs", Convert.ToBase64String(stream.ToArray()), fileName);
		}
	}

	private async Task SessionToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
	{
		if (args.Item.Id == "_sfSessionGrid_excelexport")
		{
			// Create summary items for report
			var summaryItems = new Dictionary<string, object>
			{
				{ "Total Sessions", _sessionDetailsModels?.Count ?? 0},
				{ "Active Subscriptions", _personSubscriptionModels?.Count ?? 0},
				{ "Remainig Sessions", CalculateRemainingSessionsTotal() },
				{ "Total Dues", CalculateDuesTotal() }
			};

			var hiddenColumns = new List<int>
			{
				0,
				2,
				4,
				5,
				8,
				10,
				12,
				16,
				18
			};

			// Use the generalized Excel exporter
			var stream = ExcelExportUtil.ExportToExcel(
				data: _sessionDetailsModels,
				reportTitle: "CLIENT SESSION REPORT",
				worksheetName: "Client Session Details",
				summaryItems: summaryItems);

			// Save the file with a descriptive name
			var fileName = $"Client_Session_Report.xlsx";
			await JS.InvokeVoidAsync("saveAs", Convert.ToBase64String(stream.ToArray()), fileName);
		}
	}

	private static string FormatTimeSlot(int hour)
	{
		return hour switch
		{
			< 12 => $"{hour}:00 AM",
			12 => "12:00 PM",
			_ => $"{hour - 12}:00 PM"
		};
	}

	private int CalculateRemainingSessionsTotal()
	{
		if (_personSubscriptionModels == null) return 0;
		return _personSubscriptionModels.Sum(s => s.RemainingSessions);
	}

	private decimal CalculateDuesTotal()
	{
		if (_personSubscriptionModels == null) return 0;
		return _personSubscriptionModels.Sum(s => s.Dues);
	}
}