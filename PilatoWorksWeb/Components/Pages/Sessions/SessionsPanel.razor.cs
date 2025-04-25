using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using PilatoWorksLibrary.Data;
using PilatoWorksLibrary.DataAccess;
using PilatoWorksLibrary.Models;

using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;

namespace PilatoWorksWeb.Components.Pages.Sessions;

public partial class SessionsPanel
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private UserModel _user;

	private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Now);
	private List<SlotModel> _slots = [];
	private List<SessionDetailsModel> SessionDetailsModels { get; set; } = [];

	private SfGrid<SessionDetailsModel> _sfGrid;

	private int _selectedSlotId = 1;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");

		if (firstRender)
			await LoadSlots();
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

	private async Task OnSlotChanged()
	{
		if (_selectedSlotId == 0) return;
		await LoadSessions();
	}

	private async void OnDateChanged(ChangedEventArgs<DateOnly> args)
	{
		_selectedDate = args.Value;
		await LoadSessions();
		StateHasChanged();
	}

	private async Task LoadSlots()
	{
		_slots = await CommonData.LoadTableData<SlotModel>(TableNames.Slot);
		_selectedSlotId = _slots.FirstOrDefault()?.Id ?? 1;
		await LoadSessions();
		await _sfGrid.Refresh();
		StateHasChanged();
	}

	private async Task LoadSessions()
	{
		SessionDetailsModels = await SessionData.LoadSessionDetailsByDateSlot(_selectedDate, _selectedSlotId);
		await _sfGrid.Refresh();
		StateHasChanged();
	}

	private void OnCreateClick()
	{
		Console.WriteLine("Trainer button clicked");
	}
}