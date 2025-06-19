namespace PilatoWorks.Components.Pages;

public partial class Dashboard
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private UserModel _user;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
			Dapper.SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

		if (firstRender && !await ValidatePassword())
			NavManager.NavigateTo("/Login");

		StateHasChanged();
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

		_user = user;
		return true;
	}

	private void OnSessionClick() =>
		NavManager.NavigateTo("/Sessions");

	private void OnNewUISessionClick() =>
		NavManager.NavigateTo("/New-UI-Sessions");

	private void OnTrainerClick() =>
		NavManager.NavigateTo("/Trainers");

	private void OnSubscriptionClick() =>
		NavManager.NavigateTo("/Subscriptions");

	private void OnPeopleClick() =>
		NavManager.NavigateTo("/People");

	private void OnClearDuesClick() =>
		NavManager.NavigateTo("/ClearDues");

	private void OnReportsClick() =>
		NavManager.NavigateTo("/Reports");

	private void OnTrainerManagementClick() =>
		NavManager.NavigateTo("/TrainerManagement");

	private void OnUsersClick() =>
		NavManager.NavigateTo("/UserManagement");

	private async Task OnLogoutClick()
	{
		await JS.InvokeVoidAsync("deleteCookie", "UserId");
		await JS.InvokeVoidAsync("deleteCookie", "Password");

		NavManager.NavigateTo("/");
	}

	private string GetUserRoles()
	{
		var roles = new List<string>();
		if (_user?.Session == true) roles.Add("Session Manager");
		if (_user?.Trainer == true) roles.Add("Trainer Manager");
		if (_user?.Admin == true) roles.Add("Administrator");

		return roles.Count > 0 ? string.Join(" | ", roles) : "No roles assigned";
	}
}