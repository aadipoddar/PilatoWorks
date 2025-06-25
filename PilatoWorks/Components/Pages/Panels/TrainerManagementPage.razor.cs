using Syncfusion.Blazor.Grids;

namespace PilatoWorks.Components.Pages.Panels;

public partial class TrainerManagementPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private TrainerModel _trainerModel = new();

	private List<TrainerModel> _trainerModels = [];

	private SfGrid<TrainerModel> _sfTrainerGrid;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword())
			NavManager.NavigateTo("/Login");

		if (firstRender)
			await LoadTrainersGrid();
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

	private async Task LoadTrainersGrid()
	{
		_trainerModels = await CommonData.LoadTableData<TrainerModel>(TableNames.Trainer);
		await _sfTrainerGrid.Refresh();
		StateHasChanged();
	}

	public void RowSelectHandler(RowSelectEventArgs<TrainerModel> args)
	{
		_trainerModel = args.Data;
		StateHasChanged();
	}

	private async Task OnSaveClick()
	{
		if (!await ValidateForm())
			return;

		await TrainerData.InsertTrainer(_trainerModel);

		NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
	}

	private async Task<bool> ValidateForm()
	{
		if (string.IsNullOrEmpty(_trainerModel.Name))
		{
			await JS.InvokeVoidAsync("alert", "Please enter the trainer name.");
			return false;
		}

		if (string.IsNullOrEmpty(_trainerModel.Number))
		{
			await JS.InvokeVoidAsync("alert", "Please enter a phone number.");
			return false;
		}

		if (_trainerModel.Salary < 0)
		{
			await JS.InvokeVoidAsync("alert", "Salary cannot be negative.");
			return false;
		}

		if (_trainerModel.CommissionPercent < 0 || _trainerModel.CommissionPercent > 100)
		{
			await JS.InvokeVoidAsync("alert", "Commission percentage must be between 0% and 100%.");
			return false;
		}

		return true;
	}
}
