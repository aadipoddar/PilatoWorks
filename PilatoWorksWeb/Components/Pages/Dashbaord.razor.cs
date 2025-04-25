
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using PilatoWorksLibrary.Data;
using PilatoWorksLibrary.DataAccess;
using PilatoWorksLibrary.Models;

namespace PilatoWorksWeb.Components.Pages;

public partial class Dashbaord
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private UserModel _user;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");
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
		NavManager.NavigateTo("/SessionsPanel");

	private void OnTrainerClick()
	{
		// Handle Trainer button click
		Console.WriteLine("Trainer button clicked");
	}

	private void OnSubscriptionClick()
	{
		// Handle Subscription button click
		Console.WriteLine("Subscription button clicked");
	}

	private void OnAdminClick()
	{
		// Handle Admin button click
		Console.WriteLine("Admin button clicked");
	}
}