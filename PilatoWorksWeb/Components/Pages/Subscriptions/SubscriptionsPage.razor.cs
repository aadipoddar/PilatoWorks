namespace PilatoWorksWeb.Components.Pages.Subscriptions;

public partial class SubscriptionsPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	public DateOnly ValidStartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
	public DateOnly ValidEndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

	public DateTime MinValidDate { get; set; } = DateTime.Now.AddMonths(-10);

	private UserModel _user;

	private string _personNumber;
	private string _personName;
	private string _noSessions;
	private string _booking;
	private string _cash;
	private string _card;
	private string _upi;

	private bool _status = true;
	private int _selectedSessionType = 0;

	private List<SessionTypeModel> _sessionTypes = [];

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");

		if (firstRender)
			await LoadComboBox();
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

	private async Task LoadComboBox()
	{
		_sessionTypes = await CommonData.LoadTableDataByStatus<SessionTypeModel>(TableNames.SessionType);
		_selectedSessionType = _sessionTypes.FirstOrDefault()?.Id ?? 0;
		StateHasChanged();
	}

	private async Task OnNumberChanged(string args)
	{
		_personNumber = args;
		if (_personNumber is null) return;

		var foundPerson = await PersonData.LoadPersonByNumber(_personNumber.Trim());

		if (foundPerson is not null)
		{
			_personName = foundPerson.Name;

			var validSubscriptions = await SubscriptionData.LoadValidSubscriptionByPerson(foundPerson.Id);

			if (validSubscriptions is not null)
			{
				await JS.InvokeVoidAsync("alert", "Person Already has a Valid Subscription");

				MinValidDate = validSubscriptions.SubscriptionValidTo.ToDateTime(new TimeOnly(0, 0));
				ValidStartDate = DateOnly.FromDateTime(validSubscriptions.SubscriptionValidTo.ToDateTime(new TimeOnly(0, 0)));
				ValidEndDate = DateOnly.FromDateTime(validSubscriptions.SubscriptionValidTo.ToDateTime(new TimeOnly(0, 0)).AddMonths(1));
			}

			else
			{
				MinValidDate = DateTime.Now.AddMonths(-10);
				ValidStartDate = DateOnly.FromDateTime(DateTime.Now);
				ValidEndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));
			}
		}

		else
		{
			_personName = "";

			MinValidDate = DateTime.Now.AddMonths(-10);
			ValidStartDate = DateOnly.FromDateTime(DateTime.Now);
			ValidEndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

			_personNumber = "";
			_personName = "";
			_noSessions = "";
			_booking = "";

			_selectedSessionType = 1;
			_status = true;

			_cash = "";
			_card = "";
			_upi = "";
		}

		StateHasChanged();
	}

	private async Task OnNameChanged(string args)
	{
		_personName = args;
		if (string.IsNullOrEmpty(_personName)) return;

		var foundPerson = await PersonData.LoadPersonByName(_personName.Trim());

		if (foundPerson is not null)
		{
			_personNumber = foundPerson.Number;
			_personName = foundPerson.Name;

			var validSubscriptions = await SubscriptionData.LoadValidSubscriptionByPerson(foundPerson.Id);

			if (validSubscriptions is not null)
			{
				await JS.InvokeVoidAsync("alert", "Person Already has a Valid Subscription");

				MinValidDate = validSubscriptions.SubscriptionValidTo.ToDateTime(new TimeOnly(0, 0));
				ValidStartDate = DateOnly.FromDateTime(validSubscriptions.SubscriptionValidTo.ToDateTime(new TimeOnly(0, 0)));
				ValidEndDate = DateOnly.FromDateTime(validSubscriptions.SubscriptionValidTo.ToDateTime(new TimeOnly(0, 0)).AddMonths(1));
			}
			else
			{
				MinValidDate = DateTime.Now.AddMonths(-10);
				ValidStartDate = DateOnly.FromDateTime(DateTime.Now);
				ValidEndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));
			}
		}
		else
		{
			_personNumber = "";

			MinValidDate = DateTime.Now.AddMonths(-10);
			ValidStartDate = DateOnly.FromDateTime(DateTime.Now);
			ValidEndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

			_noSessions = "";
			_booking = "";

			_selectedSessionType = 1;
			_status = true;

			_cash = "";
			_card = "";
			_upi = "";
		}

		StateHasChanged();
	}


	private async Task OnSaveClick()
	{
		if (!await ValidateForm())
			return;

		var person = await PersonData.LoadPersonByNumber(_personNumber.Trim());
		if (person is null)
		{
			await JS.InvokeVoidAsync("alert", "Person not found.");
			return;
		}

		var subscription = new SubscriptionModel
		{
			Id = 0,
			PersonId = person.Id,
			ValidFrom = ValidStartDate,
			ValidTo = ValidEndDate,
			SessionTypeId = _selectedSessionType,
			NoSessions = int.Parse(_noSessions.Trim()),
			Booking = int.Parse(_booking.Trim()),
			UserId = _user.Id,
			Status = _status,
			SubscriptionDate = DateTime.Now
		};

		subscription.Id = await SubscriptionData.InsertSubscription(subscription);
		if (subscription.Id == 0)
		{
			await JS.InvokeVoidAsync("alert", "Error in creating subscription.");
			return;
		}

		for (int i = 1; i <= 3; i++)
		{
			int amount = int.Parse(i switch
			{
				1 => _cash,
				2 => _card,
				3 => _upi,
				_ => "0"
			});

			// Skip insertion if the amount is zero
			if (amount == 0)
				continue;

			await SubscriptionData.InsertSubscriptionPaymentDetails(new SubscriptionPaymentDetailsModel
			{
				Id = 0,
				PaymentModeId = i,
				SubscriptionId = subscription.Id,
				Amount = amount,
				PaymentDate = DateTime.Now,
				Status = true
			});
		}

		NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
	}

	private async Task<bool> ValidateForm()
	{
		if (string.IsNullOrEmpty(_personNumber) || string.IsNullOrEmpty(_personNumber))
		{
			await JS.InvokeVoidAsync("alert", "Please enter a valid Person Number");
			return false;
		}

		if (string.IsNullOrEmpty(_noSessions))
		{
			await JS.InvokeVoidAsync("alert", "Please enter a valid number of sessions.");
			return false;
		}

		if (string.IsNullOrEmpty(_booking))
		{
			await JS.InvokeVoidAsync("alert", "Please enter a valid booking amount.");
			return false;
		}

		if (ValidStartDate > ValidEndDate)
		{
			await JS.InvokeVoidAsync("alert", "Start date cannot be greater than end date.");
			return false;
		}

		if (string.IsNullOrEmpty(_cash)) _cash = "0";
		if (string.IsNullOrEmpty(_card)) _card = "0";
		if (string.IsNullOrEmpty(_upi)) _upi = "0";

		if (int.Parse(_cash) + int.Parse(_card) + int.Parse(_upi) > int.Parse(_booking.Trim()))
		{
			await JS.InvokeVoidAsync("alert", "Amount Collected is greater than booking Amount.");
			return false;
		}

		var person = await PersonData.LoadPersonByNumber(_personNumber.Trim());
		if (person is null)
		{
			await JS.InvokeVoidAsync("alert", "Person not found.");
			return false;
		}

		// Check for overlapping subscriptions
		var existingSubscriptions = await SubscriptionData.LoadSubscriptionByPerson(person.Id);
		foreach (var subscription in existingSubscriptions)
		{
			// Check if new subscription dates overlap with any existing subscription
			if (ValidStartDate <= subscription.SubscriptionValidTo && ValidEndDate >= subscription.SubscriptionValidFrom)
			{
				await JS.InvokeVoidAsync("alert",
					$"This person already has a subscription that overlaps with the selected date range " +
					$"(existing subscription: {subscription.SubscriptionValidFrom} to {subscription.SubscriptionValidTo})");
				return false;
			}
		}

		return true;
	}

	private void OnCancelClick() =>
		NavManager.NavigateTo("/");
}