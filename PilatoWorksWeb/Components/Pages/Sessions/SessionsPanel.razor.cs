using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using PilatoWorksLibrary.Data;
using PilatoWorksLibrary.DataAccess;
using PilatoWorksLibrary.Models;

using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;

namespace PilatoWorksWeb.Components.Pages.Sessions;

public partial class SessionsPanel
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private UserModel _user;

	private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Now);

	private int _selectedSlot = 0;
	private int _selectedValidSub = 0;
	private int _selectedTrainer1 = 0;
	private int _selectedTrainer2 = -1;

	private bool _isUpdating = false;
	private bool _isConfirmed = true;

	private List<SlotModel> _slots = [];
	private List<TrainerModel> _trainers = [];
	private List<ValidSubscriptionModel> _validSubs = [];

	private SessionDetailsModel _selectedSession = null;


	private List<SessionDetailsModel> SessionDetailsModels { get; set; } = [];

	private SfGrid<SessionDetailsModel> _sfGrid;


	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");

		if (firstRender)
		{
			await LoadSlots();
			await LoadComboBox();
		}
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

	private async void OnSlotChanged(ChangeEventArgs<int, SlotModel> args)
	{
		if (_selectedSlot == 0) return;
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
		_selectedSlot = _slots.FirstOrDefault()?.Id ?? 1;
		await LoadSessions();
		await _sfGrid.Refresh();
		StateHasChanged();
	}

	private async Task LoadSessions()
	{
		SessionDetailsModels = await SessionData.LoadSessionDetailsByDateSlot(_selectedDate, _selectedSlot);
		await _sfGrid.Refresh();
		await LoadValidPersons();
		StateHasChanged();
	}

	private async Task LoadValidPersons()
	{
		_validSubs = await SubscriptionData.LoadValidSubscriptionByDate(_selectedDate);
		_validSubs = [.. _validSubs
			.GroupBy(sub => sub.PersonId)
			.Select(group => group.First())];

		var slotSessions = await SessionData.LoadSessionDetailsByDateSlot(_selectedDate, _selectedSlot);

		foreach (var slotSession in slotSessions)
		{
			var subscription = _validSubs.FirstOrDefault(sub => sub.SubscriptionId == slotSession.SubscriptionId);
			if (subscription is not null)
				_validSubs.Remove(subscription);
		}

		_selectedValidSub = _validSubs.FirstOrDefault()?.SubscriptionId ?? 1;
		StateHasChanged();
	}

	private async Task LoadComboBox()
	{
		_trainers = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);
		_trainers.Add(new TrainerModel { Id = -1, Name = "No Trainer" });
		_selectedTrainer1 = _trainers.FirstOrDefault()?.Id ?? 1;
		_selectedTrainer2 = -1;

		StateHasChanged();
	}

	public void RowSelectHandler(RowSelectEventArgs<SessionDetailsModel> args)
	{
		_isUpdating = true;

		_selectedSession = args.Data;
		if (_selectedSession is not null)
		{
			_selectedTrainer1 = _selectedSession.Trainer1Id;
			_selectedTrainer2 = _selectedSession.Trainer2Id ?? -1;
			_isConfirmed = _selectedSession.Confirmed;
		}

		StateHasChanged();
	}

	private async Task OnSaveClick()
	{
		if (!await ValidateForm())
			return;

		await SessionData.InsertSession(new SessionModel
		{
			Id = _selectedSession?.Id ?? 0,
			SessionDate = _selectedSession?.SessionDate ?? _selectedDate,
			SlotId = _selectedSession?.SlotId ?? _selectedSlot,
			SubscriptionId = _selectedSession?.SubscriptionId ?? _selectedValidSub,
			CreatedDate = _selectedSession?.CreatedDate ?? DateTime.Now,
			UserId = _selectedSession?.UserId ?? _user.Id,
			Trainer1Id = _selectedTrainer1,
			Trainer2Id = _selectedTrainer2 == -1 ? null : _selectedTrainer2,
			Confirmed = _isConfirmed,
		});

		_isUpdating = false;
		_selectedSession = null;
		_isConfirmed = true;
		await LoadSessions();
	}

	private async Task<bool> ValidateForm()
	{
		if (_selectedTrainer1 == 0 || _selectedTrainer1 == -1)
		{
			await JS.InvokeVoidAsync("alert", "Please select a trainer.");
			return false;
		}
		if (_selectedValidSub == 0)
		{
			await JS.InvokeVoidAsync("alert", "Please select a valid Person.");
			return false;
		}

		if (_selectedTrainer2 == _selectedTrainer1)
		{
			await JS.InvokeVoidAsync("alert", "Trainer 1 and Trainer 2 cannot be the same.");
			return false;
		}
		return true;
	}
}