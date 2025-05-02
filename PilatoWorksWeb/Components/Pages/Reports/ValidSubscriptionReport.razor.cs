using Syncfusion.Blazor.Grids;

namespace PilatoWorksWeb.Components.Pages.Reports;

public partial class ValidSubscriptionReport
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private readonly string[] _pageSizes = ["5", "10", "15", "20"];

	private List<SubscriptionDetailsModel> _validSubscriptionModels = [];
	private SfGrid<SubscriptionDetailsModel> _sfSubscriptionGrid;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");

		if (firstRender)
			await LoadData();
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

	private async Task LoadData()
	{
		_validSubscriptionModels = await CommonData.LoadTableData<SubscriptionDetailsModel>(ViewNames.ValidSubscriptionsDetails);
		await _sfSubscriptionGrid.Refresh();
		StateHasChanged();
	}

	private void OnBackClick() =>
		NavManager.NavigateTo("/Reports");

	private async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
	{
		if (args.Item.Id == "_sfSubscriptionGrid_excelexport")
		{
			var summaryItems = new Dictionary<string, object>
			{
				{ "Active Subscriptions", _validSubscriptionModels.Count },
				{ "Total Sessions", CalculateTotalSessions() },
				{ "Remaining Sessions", _validSubscriptionModels.Sum(s => s.RemainingSessions) },
				{ "Total Revenue", CalculateTotalRevenue() },
				{ "Outstanding Dues", CalculateTotalDues() }
			};

			// Use the generalized Excel exporter
			var stream = ExcelExportUtil.ExportToExcel(
				data: _validSubscriptionModels,
				reportTitle: "ACTIVE SUBSCRIPTION DETAIL REPORT",
				worksheetName: "Active Subscription Details",
				summaryItems: summaryItems);

			// Save the file with a descriptive name
			var fileName = $"Active_Subscription_Report.xlsx";
			await JS.InvokeVoidAsync("saveAs", Convert.ToBase64String(stream.ToArray()), fileName);
		}
	}

	private int CalculateTotalSessions()
	{
		if (_validSubscriptionModels == null) return 0;
		return _validSubscriptionModels.Sum(s => s.NoOfSessions);
	}

	private decimal CalculateTotalRevenue()
	{
		if (_validSubscriptionModels == null) return 0;
		return _validSubscriptionModels.Sum(s => s.TotalPaid);
	}

	private decimal CalculateTotalDues()
	{
		if (_validSubscriptionModels == null) return 0;
		return _validSubscriptionModels.Sum(s => s.Dues);
	}
}