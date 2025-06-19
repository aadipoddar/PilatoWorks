using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;

namespace PilatoWorks.Components.Pages.Reports;

public partial class SessionsReport
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	public DateOnly SessionsStartDate { get; set; }
	public DateOnly SessionsEndDate { get; set; }

	private readonly string[] _pageSizes = ["5", "10", "15", "20"];

	private List<SessionDetailsModel> _sessionModels = [];
	private SfGrid<SessionDetailsModel> _sfSessionGrid;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword())
			NavManager.NavigateTo("/Login");

		if (firstRender)
		{
			SessionsStartDate = DateOnly.FromDateTime(DateTime.Today);
			SessionsEndDate = DateOnly.FromDateTime(DateTime.Today);

			await LoadSessionData();
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

	private void OnBackClick() =>
		NavManager.NavigateTo("/Reports");

	private async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
	{
		if (args.Item.Id == "_sfSessionGrid_excelexport")
		{
			var summaryItems = new Dictionary<string, object>
			{
				{ "Total Sessions", _sessionModels.Count },
				{ "Unique Clients", _sessionModels?.Select(s => s.PersonName).Distinct().Count() ?? 0 },
				{ "Confirmed Sessions", _sessionModels?.Count(s => s.Confirmed) ?? 0 }
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
				data: _sessionModels,
				reportTitle: "SESSIONS DETAIL REPORT",
				worksheetName: "Session Details",
				dateRangeStart: SessionsStartDate,
				dateRangeEnd: SessionsEndDate,
				summaryItems: summaryItems,
				hiddenColumns: hiddenColumns);

			// Save the file with a descriptive name
			var fileName = $"Session_Report_{SessionsStartDate:yyyy-MM-dd}_to_{SessionsEndDate:yyyy-MM-dd}.xlsx";
			await JS.InvokeVoidAsync("saveAs", Convert.ToBase64String(stream.ToArray()), fileName);
		}
	}

	private async Task LoadSessionData()
	{
		_sessionModels = await SessionData.LoadSessionDetailsByDateRange(SessionsStartDate, SessionsEndDate);
		await _sfSessionGrid.Refresh();
		StateHasChanged();
	}

	private async Task SessionDateChanged(RangePickerEventArgs<DateOnly> args)
	{
		SessionsStartDate = args.StartDate;
		SessionsEndDate = args.EndDate;
		await LoadSessionData();
		StateHasChanged();
	}
}