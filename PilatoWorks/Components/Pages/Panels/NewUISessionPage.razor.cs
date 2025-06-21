using Syncfusion.Blazor.Calendars;

namespace PilatoWorks.Components.Pages.Panels;

public partial class NewUISessionPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private UserModel _user;

	private bool _isConfirmDialogVisible = false;
	private bool _isSessionDialogVisible = false;
	private bool _isCopyDialogVisible = false;
	private bool _isNotInsertedDialogVisible = false;

	private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Now);
	private DateOnly _copySourceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));

	private SlotModel _selectedSlot;
	private SessionDetailsModel _selectedSession = new() { Confirmed = true };

	private List<SlotModel> _slots = [];
	private List<TrainerModel> _trainers = [];
	private List<SessionDetailsModel> _sessions = [];
	private List<SubscriptionDetailsModel> _validSubs = [];
	private List<SessionDetailsModel> _notInsertedSessions = [];

	#region Load Data
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword())
			NavManager.NavigateTo("/Login");

		if (firstRender)
			await LoadInitialData();
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

	private async Task LoadInitialData()
	{
		_slots = await CommonData.LoadTableData<SlotModel>(TableNames.Slot);
		await LoadSessions();

		_trainers = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);

		StateHasChanged();
	}

	private async void OnDateChanged(ChangedEventArgs<DateOnly> args)
	{
		_selectedDate = args.Value;
		await LoadSessions();
		StateHasChanged();
	}

	private async Task LoadSessions()
	{
		_sessions = await SessionData.LoadSessionDetailsByDateRange(_selectedDate, _selectedDate);
		StateHasChanged();
	}

	private async Task LoadValidPersons()
	{
		_validSubs = await SubscriptionData.LoadValidSubscriptionByDate(_selectedDate);
		_validSubs = [.. _validSubs
			.GroupBy(sub => sub.PersonId)
			.Select(group => group.First())];

		if (_selectedSlot is null || _selectedSlot.Id <= 0)
			return;

		var slotSessions = await SessionData.LoadSessionDetailsByDateSlot(_selectedDate, _selectedSlot.Id);

		foreach (var slotSession in slotSessions)
		{
			if (_selectedSession.Id > 0)
				if (slotSession.Id == _selectedSession.Id)
					continue;

			var subscription = _validSubs.FirstOrDefault(sub => sub.SubscriptionId == slotSession.SubscriptionId);
			if (subscription is not null)
				_validSubs.Remove(subscription);
		}

		StateHasChanged();
	}
	#endregion

	#region Dialog Events
	private async Task CreateNewSessionClick(SlotModel slot)
	{
		var sessions = await SessionData.LoadSessionDetailsByDateSlot(_selectedDate, slot.Id);

		_selectedSlot = slot;
		_selectedSession = new() { Confirmed = true };

		await LoadValidPersons();

		if (slot.Max <= sessions.Where(c => c.Confirmed).Count())
		{
			_isConfirmDialogVisible = true;
			return;
		}

		_isSessionDialogVisible = true;
	}

	private async Task OnSessionClick(SessionDetailsModel session)
	{
		_selectedSession = session;
		_selectedSlot = _slots.FirstOrDefault(s => s.Id == session.SlotId);

		await LoadValidPersons();

		_isSessionDialogVisible = true;
		StateHasChanged();
	}

	private void ConfirmDialogOkClick()
	{
		_isConfirmDialogVisible = false;
		_isSessionDialogVisible = true;
	}

	private void ConfirmDialogCancelClick() =>
		_isConfirmDialogVisible = false;

	private async Task DeleteSessionClick()
	{
		if (_selectedSession.Id > 0)
		{
			await SessionData.DeleteSessionById(_selectedSession.Id);

			_isSessionDialogVisible = false;

			_selectedSession = new() { Confirmed = true };
			_selectedSlot = null;

			await LoadSessions();

			StateHasChanged();
		}
	}

	private void OnCopySessionsClick() =>
		_isCopyDialogVisible = true;

	private async Task CopyScheduleClick()
	{
		_isCopyDialogVisible = false;

		await LoadSessions();

		foreach (var session in _sessions)
			await SessionData.DeleteSessionById(session.Id);

		_notInsertedSessions = [];
		await LoadValidPersons();
		var sessions = await SessionData.LoadSessionDetailsByDateRange(_copySourceDate, _copySourceDate);

		foreach (var session in sessions)
		{
			if (!_validSubs.Any(s => s.SubscriptionId == session.SubscriptionId))
			{
				_notInsertedSessions.Add(session);
				continue;
			}

			await SessionData.InsertSession(new()
			{
				Id = 0,
				SessionDate = _selectedDate,
				SubscriptionId = session.SubscriptionId,
				Trainer1Id = session.Trainer1Id,
				Trainer2Id = session.Trainer2Id,
				UserId = _user.Id,
				SlotId = session.SlotId,
				Confirmed = session.Confirmed,
				CreatedDate = DateTime.Now,
			});
		}

		await LoadValidPersons();
		await LoadSessions();

		if (_notInsertedSessions.Count != 0)
			_isNotInsertedDialogVisible = true;

		StateHasChanged();
	}
	#endregion

	#region Saving
	private async Task SessionDialogOkClick()
	{
		if (_selectedSession.SlotId <= 0)
			_selectedSession.SlotId = _selectedSlot.Id;

		if (_selectedSession.Trainer1Id <= 0 && _selectedSession.Trainer2Id > 0 && _selectedSession.Trainer2Id is not null)
		{
			_selectedSession.Trainer1Id = _selectedSession.Trainer2Id.Value;
			_selectedSession.Trainer2Id = null;
		}

		if (_selectedSession.SubscriptionId <= 0 ||
			(_selectedSession.Trainer1Id <= 0 && _selectedSession.Trainer2Id <= 0) ||
			_selectedSession.Id < 0 ||
			_selectedSession.Trainer1Id == _selectedSession.Trainer2Id)
		{
			_isSessionDialogVisible = false;
			_selectedSession = new() { Confirmed = true };
			_selectedSlot = null;

			await LoadSessions();

			StateHasChanged();

			return;
		}

		await SessionData.InsertSession(new()
		{
			Id = _selectedSession.Id,
			SessionDate = _selectedDate,
			SubscriptionId = _selectedSession.SubscriptionId,
			Trainer1Id = _selectedSession.Trainer1Id,
			Trainer2Id = _selectedSession.Trainer2Id,
			UserId = _user.Id,
			SlotId = _selectedSlot.Id,
			Confirmed = _selectedSession.Confirmed,
			CreatedDate = DateTime.Now,
		});

		_isSessionDialogVisible = false;
		_selectedSession = new() { Confirmed = true };
		_selectedSlot = null;

		await LoadSessions();

		StateHasChanged();
	}
	#endregion

	private void OnVoiceAssistantClick()
	{
		// Implement voice assistant functionality here
	}
}