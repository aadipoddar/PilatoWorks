using Syncfusion.Blazor.Grids;

namespace PilatoWorksWeb.Components.Pages.Reports;

public partial class TrainerReport
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private readonly string[] _pageSizes = ["5", "10", "15", "20"];

	private List<TrainerModel> _trainerModels = [];
	private SfGrid<TrainerModel> _sfTrainerGrid;

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
		_trainerModels = await CommonData.LoadTableData<TrainerModel>(TableNames.Trainer);
		await _sfTrainerGrid.Refresh();
		StateHasChanged();
	}

	private void OnBackClick() =>
		NavManager.NavigateTo("/Reports");

	private async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
	{
		if (args.Item.Id == "_sfTrainerGrid_excelexport")
			await _sfTrainerGrid.ExportToExcelAsync();
		else if (args.Item.Id == "_sfTrainerGrid_pdfexport")
			await _sfTrainerGrid.ExportToPdfAsync();
	}

	private int CalculateActiveTrainers()
	{
		if (_trainerModels == null) return 0;
		return _trainerModels.Count(t => t.Status && t.Status.ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase));
	}

	private decimal CalculateTotalSalary()
	{
		if (_trainerModels == null) return 0;
		return _trainerModels.Sum(t => t.Salary);
	}

	private decimal CalculateAvgCommission()
	{
		if (_trainerModels == null || _trainerModels.Count == 0) return 0;
		return Math.Round(_trainerModels.Average(t => t.CommissionPercent), 2);
	}
}