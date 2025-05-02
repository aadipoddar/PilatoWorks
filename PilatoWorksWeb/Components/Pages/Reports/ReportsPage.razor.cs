namespace PilatoWorksWeb.Components.Pages.Reports;

public partial class ReportsPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");
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

	private void OnPeopleReportClick() =>
		NavManager.NavigateTo("/PeopleReport");

	private void OnTrainerReportClick() =>
		NavManager.NavigateTo("/TrainerReport");

	private void OnValidSubscriptionReportClick() =>
		NavManager.NavigateTo("/ValidSubscriptionReport");

	private void OnSubscriptionDetailsClick() =>
		NavManager.NavigateTo("/SubscriptionDetailReport");

	private void OnSessionsReportClick() =>
		NavManager.NavigateTo("/SessionsReport");

	private void NavigateToDashboard() =>
		NavManager.NavigateTo("/dashboard");

}