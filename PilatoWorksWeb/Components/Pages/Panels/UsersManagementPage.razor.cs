
using Syncfusion.Blazor.Grids;

namespace PilatoWorksWeb.Components.Pages.Panels;

public partial class UsersManagementPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private UserModel _userModel = new();

	private List<UserModel> _userModels = [];

	private SfGrid<UserModel> _sfUserGrid;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");

		if (firstRender)
			await LoadUsersGrid();
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

	private async Task LoadUsersGrid()
	{
		_userModels = await CommonData.LoadTableData<UserModel>(TableNames.User);
		await _sfUserGrid.Refresh();
		StateHasChanged();
	}

	public void RowSelectHandler(RowSelectEventArgs<UserModel> args)
	{
		_userModel = args.Data;
		StateHasChanged();
	}

	private async Task OnSaveClick()
	{
		if (!await ValidateForm())
			return;

		await UserData.InsertUser(_userModel);

		NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
	}

	private async Task<bool> ValidateForm()
	{
		if (string.IsNullOrEmpty(_userModel.Name))
		{
			await JS.InvokeVoidAsync("alert", "Please enter a name.");
			return false;
		}

		if (string.IsNullOrEmpty(_userModel.Password.ToString()))
		{
			await JS.InvokeVoidAsync("alert", "Please enter a password.");
			return false;
		}

		if (_userModel.Password.ToString().Length != 4)
		{
			await JS.InvokeVoidAsync("alert", "Password must be of 4 characters.");
			return false;
		}

		if (_userModel.Admin)
		{
			_userModel.Session = true;
			_userModel.Trainer = true;
		}

		var user = await UserData.LoadUserByPassword(_userModel.Password.ToString().RemoveSpace());
		if (user is not null)
		{
			await JS.InvokeVoidAsync("alert", "Users Cannot have the same Password.");
			return false;
		}

		return true;
	}

	private void OnCancelClick() =>
		NavManager.NavigateTo("/");
}