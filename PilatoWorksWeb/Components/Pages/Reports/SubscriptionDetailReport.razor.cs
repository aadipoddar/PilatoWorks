using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;

namespace PilatoWorksWeb.Components.Pages.Reports;

public partial class SubscriptionDetailReport
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	public DateOnly SubscriptionsStartDate { get; set; }
	public DateOnly SubscriptionsEndDate { get; set; }

	private readonly string[] _pageSizes = ["5", "10", "15", "20"];

	private List<ValidSubscriptionModel> _subscriptionModels = [];
	private SfGrid<ValidSubscriptionModel> _sfSubscriptionGrid;

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
			await _sfSubscriptionGrid?.ExportToExcelAsync();
		else if (args.Item.Id == "_sfSubscriptionGrid_pdfexport")
			await _sfSubscriptionGrid?.ExportToPdfAsync();
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
}
