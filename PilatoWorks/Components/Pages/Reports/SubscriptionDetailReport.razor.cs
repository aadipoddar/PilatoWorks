using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;

namespace PilatoWorks.Components.Pages.Reports;

public partial class SubscriptionDetailReport
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	public DateOnly SubscriptionsStartDate { get; set; }
	public DateOnly SubscriptionsEndDate { get; set; }

	private readonly string[] _pageSizes = ["5", "10", "15", "20"];

	private List<SubscriptionDetailsModel> _subscriptionModels = [];
	private SfGrid<SubscriptionDetailsModel> _sfSubscriptionGrid;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");

		if (firstRender)
		{
			var today = DateOnly.FromDateTime(DateTime.Today);
			SubscriptionsStartDate = new DateOnly(today.Year, today.Month, 1);
			SubscriptionsEndDate = new DateOnly(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

			await LoadSubscriptionData();
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
		if (args.Item.Id == "_sfSubscriptionGrid_excelexport")
		{
			// Create summary items for report
			var summaryItems = new Dictionary<string, object>
			{
				{ "Total Subscriptions", _subscriptionModels.Count },
				{ "Total Sessions", CalculateTotalSessions() },
				{ "Remaining Sessions", _subscriptionModels.Sum(s => s.RemainingSessions) },
				{ "Total Revenue", CalculateTotalRevenue() },
				{ "Outstanding Dues", CalculateTotalDues() }
			};

			var hiddenColumns = new List<int>
			{
				0,
				3,
				7,
				13,
				14,
				15
			};

			// Use the generalized Excel exporter
			var stream = ExcelExportUtil.ExportToExcel(
				data: _subscriptionModels,
				reportTitle: "SUBSCRIPTION DETAIL REPORT",
				worksheetName: "Subscription Details",
				dateRangeStart: SubscriptionsStartDate,
				dateRangeEnd: SubscriptionsEndDate,
				summaryItems: summaryItems,
				hiddenColumns: hiddenColumns);

			// Save the file with a descriptive name
			var fileName = $"Subscription_Report_{SubscriptionsStartDate:yyyy-MM-dd}_to_{SubscriptionsEndDate:yyyy-MM-dd}.xlsx";
			await JS.InvokeVoidAsync("saveAs", Convert.ToBase64String(stream.ToArray()), fileName);
		}
	}

	private async Task LoadSubscriptionData()
	{
		_subscriptionModels = await SubscriptionData.LoadSubscriptionByDateRange(SubscriptionsStartDate, SubscriptionsEndDate);
		await _sfSubscriptionGrid.Refresh();
		StateHasChanged();
	}

	private async Task SubscriptionDateChanged(RangePickerEventArgs<DateOnly> args)
	{
		SubscriptionsStartDate = args.StartDate;
		SubscriptionsEndDate = args.EndDate;
		await LoadSubscriptionData();
		StateHasChanged();
	}

	private int CalculateTotalSessions()
	{
		if (_subscriptionModels == null) return 0;
		return _subscriptionModels.Sum(s => s.NoOfSessions);
	}

	private decimal CalculateTotalRevenue()
	{
		if (_subscriptionModels == null) return 0;
		return _subscriptionModels.Sum(s => s.TotalPaid);
	}

	private decimal CalculateTotalDues()
	{
		if (_subscriptionModels == null) return 0;
		return _subscriptionModels.Sum(s => s.Dues);
	}

	public void SubscriptionDetailRowSelected(RowSelectEventArgs<SubscriptionDetailsModel> args) =>
		NavManager.NavigateTo($"/Subscriptions/{args.Data.SubscriptionId}");
}
