using Syncfusion.Blazor.Grids;

namespace PilatoWorksWeb.Components.Pages.Panels;

public partial class ReportsPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private List<PersonModel> _personModels = [];
	private List<ValidSubscriptionModel> _validSubscriptionModels = [];
	private List<TrainerModel> _trainerModels = [];

	private SfGrid<PersonModel> _sfPersonGrid;
	private SfGrid<ValidSubscriptionModel> _sfSubscriptionGrid;
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

	private int _currentTab = 1;

	private void SelectTab(int tabNumber)
	{
		_currentTab = tabNumber;
	}

	private string GetTabClass(int tabNumber)
	{
		return tabNumber == _currentTab ? "tab-button active" : "tab-button";
	}

	private async Task LoadData()
	{
		_personModels = await CommonData.LoadTableData<PersonModel>(TableNames.Person);
		_validSubscriptionModels = await CommonData.LoadTableData<ValidSubscriptionModel>(ViewNames.ValidSubscriptionsDetails);
		_trainerModels = await CommonData.LoadTableData<TrainerModel>(TableNames.Trainer);

		await _sfPersonGrid.Refresh();
		await _sfSubscriptionGrid.Refresh();
		await _sfTrainerGrid.Refresh();
		StateHasChanged();
	}
}