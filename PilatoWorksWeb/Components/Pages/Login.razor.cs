using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using PilatoWorksLibrary.Data;
using PilatoWorksLibrary.DataAccess;
using PilatoWorksLibrary.Models;

namespace PilatoWorksWeb.Components.Pages;

public partial class Login
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private string _password = "";

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
			Dapper.SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

		var userId = await JS.InvokeAsync<string>("getCookie", "UserId");
		var password = await JS.InvokeAsync<string>("getCookie", "Password");

		if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password))
		{
			var user = await CommonData.LoadTableDataById<UserModel>(TableNames.User, int.Parse(userId));
			if (user is null) return;

			if (BCrypt.Net.BCrypt.EnhancedVerify(user.Password.ToString(), password))
				NavManager.NavigateTo("/Dashboard");
		}
	}

	private async void CheckPassword(ChangeEventArgs e)
	{
		_password = e.Value?.ToString() ?? string.Empty;
		if (_password.Length != 4) return;

		var user = await UserData.LoadUserByPassword(_password);
		if (user is null) return;

		await JS.InvokeVoidAsync("setCookie", "UserId", user.Id, 1);
		await JS.InvokeVoidAsync("setCookie", "Password", BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password.ToString(), 13), 1);

		NavManager.NavigateTo("/Dashboard");
	}
}