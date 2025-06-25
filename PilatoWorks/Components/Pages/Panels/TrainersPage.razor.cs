using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;

namespace PilatoWorks.Components.Pages.Panels;

public partial class TrainersPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private TrainerModel _trainer = new();

	private string _noSessions;
	private string _totalSales;

	public DateOnly _sessionStartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
	public DateOnly _sessionEndDate = DateOnly.FromDateTime(DateTime.Now);

	private List<TrainerModel> _trainers = [];
	private List<SessionDetailsModel> _sessions = [];

	private SfGrid<SessionDetailsModel> _sfGrid;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword())
			NavManager.NavigateTo("/Login");

		if (firstRender)
			_trainers = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);

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

		return true;
	}

	private async Task OnTrainerSelected(int trainerId)
	{
		_trainer = _trainers.FirstOrDefault(x => x.Id == trainerId);

		await LoadSessions();
		StateHasChanged();
	}

	private async Task SessionDateChanged(RangePickerEventArgs<DateOnly> args)
	{
		_sessionStartDate = args.StartDate;
		_sessionEndDate = args.EndDate;

		await LoadSessions();
	}

	private async Task LoadSessions()
	{
		if (_trainer is null || _sessionStartDate > _sessionEndDate)
			_sessions.Clear();

		else
			_sessions = await SessionData.LoadSessionDetailsByDateTrainer(
				_sessionStartDate,
				_sessionEndDate,
				_trainer.Id);

		await _sfGrid.Refresh();

		_noSessions = _sessions.Count.ToString();
		_totalSales = _sessions.Sum(x => x.PerSessionCost).FormatIndianCurrency();

		StateHasChanged();
	}
}