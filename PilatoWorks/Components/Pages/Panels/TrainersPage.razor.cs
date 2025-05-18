using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;

namespace PilatoWorks.Components.Pages.Panels;

public partial class TrainersPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	public DateOnly SessionStartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
	public DateOnly SessionEndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

	private List<SessionDetailsModel> SessionDetailsModels { get; set; } = [];

	private UserModel _user;

	private SfGrid<SessionDetailsModel> _sfGrid;

	private TrainerModel _foundTrainer = null;

	private string _trainerNumber;
	private string _trainerName;
	private string _trainerAddress;
	private string _trainerSalary;
	private string _trainerCommissionPercent;
	private string _noSessions;
	private string _totalSales;

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

		_user = user;
		return true;
	}

	private async Task OnNumberChanged(string args)
	{
		_trainerNumber = args;
		if (_trainerNumber is null) return;

		var trainers = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);
		_foundTrainer = trainers.FirstOrDefault(x => x.Number == _trainerNumber.Trim().RemoveSpace());

		if (_foundTrainer is not null)
		{
			_trainerName = _foundTrainer.Name;
			_trainerAddress = _foundTrainer.Address;
			_trainerSalary = _foundTrainer.Salary.FormatIndianCurrency();
			_trainerCommissionPercent = _foundTrainer.CommissionPercent.ToString();
		}

		else
		{
			_trainerNumber = null;
			_trainerName = null;
			_trainerAddress = null;
			_trainerSalary = null;
			_trainerCommissionPercent = null;
		}

		await LoadSessions();
		StateHasChanged();
	}

	private async Task SessionDateChanged(RangePickerEventArgs<DateOnly> args)
	{
		SessionStartDate = args.StartDate;
		SessionEndDate = args.EndDate;

		await LoadSessions();
	}

	private async Task LoadSessions()
	{
		if (_foundTrainer is null || SessionStartDate > SessionEndDate)
			SessionDetailsModels.Clear();

		else
			SessionDetailsModels = await SessionData.LoadSessionDetailsByDateTrainer(
				SessionStartDate,
				SessionEndDate,
				_foundTrainer.Id);

		await _sfGrid.Refresh();

		_noSessions = SessionDetailsModels.Count.ToString();
		_totalSales = SessionDetailsModels.Sum(x => x.PerSessionCost).FormatIndianCurrency();

		StateHasChanged();
	}
}