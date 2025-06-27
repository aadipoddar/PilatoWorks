namespace PilatoWorks.Components.Pages.Subscriptions;

public partial class SubscriptionsPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	[Parameter] public int? SubscriptionId { get; set; }

	private UserModel _user;

	private int _cash;
	private int _card;
	private int _upi;

	private DateTime _minValidDate = DateTime.Now.AddMonths(-10);

	private PersonModel _person;
	private SubscriptionModel _subscription = new()
	{
		Id = 0,
		NoSessions = 1,
		Booking = 0,
		ValidFrom = DateOnly.FromDateTime(DateTime.Now),
		ValidTo = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
		SubscriptionDate = DateTime.Now.AddHours(5).AddMinutes(30),
		Status = true,
	};

	private List<PersonModel> _persons = [];
	private List<SessionTypeModel> _sessionTypes = [];

	#region Load Data
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword())
			NavManager.NavigateTo("/Login");

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
		_persons = await CommonData.LoadTableData<PersonModel>(TableNames.Person);
		_sessionTypes = await CommonData.LoadTableDataByStatus<SessionTypeModel>(TableNames.SessionType);

		_subscription.SessionTypeId = _sessionTypes.LastOrDefault()?.Id ?? 2;

		if (SubscriptionId.HasValue && SubscriptionId.Value > 0)
			await LoadSubscription();

		StateHasChanged();
	}

	private async Task LoadSubscription()
	{
		_subscription = await CommonData.LoadTableDataById<SubscriptionModel>(TableNames.Subscription, SubscriptionId.Value);
		if (_subscription is null)
		{
			await JS.InvokeVoidAsync("alert", "Subscription not found.");
			NavManager.NavigateTo("/");
			return;
		}

		_person = await CommonData.LoadTableDataById<PersonModel>(TableNames.Person, _subscription.PersonId);
		if (_person is null)
		{
			await JS.InvokeVoidAsync("alert", "Person not found for the subscription.");
			NavManager.NavigateTo("/");
			return;
		}

		var payments = await SubscriptionData.LoadSubscriptionPaymentDetailsBySubscriptionId(_subscription.Id);
		if (payments is not null)
		{
			_cash = payments.FirstOrDefault(p => p.PaymentModeId == 1)?.Amount ?? 0;
			_card = payments.FirstOrDefault(p => p.PaymentModeId == 2)?.Amount ?? 0;
			_upi = payments.FirstOrDefault(p => p.PaymentModeId == 3)?.Amount ?? 0;
		}
		else
		{
			_cash = 0;
			_card = 0;
			_upi = 0;
		}

		StateHasChanged();
	}

	private async Task OnPersonSelected(int? personId)
	{
		if (personId is null || personId == 0)
			NavManager.NavigateTo(NavManager.Uri, forceLoad: true);

		if (personId is not null)
		{
			_person = _persons.FirstOrDefault(p => p.Id == personId.Value);

			var validSubscriptions = await SubscriptionData.LoadValidSubscriptionByPerson(_person.Id);
			if (validSubscriptions is not null)
			{
				await JS.InvokeVoidAsync("alert", "Person Already has a Valid Subscription");

				_minValidDate = validSubscriptions.SubscriptionValidTo.ToDateTime(new TimeOnly(0, 0)).AddDays(1);

				_subscription = new()
				{
					Id = 0,
					PersonId = _person.Id,
					NoSessions = 1,
					Booking = 0,
					SessionTypeId = validSubscriptions.SessionTypeId,
					ValidFrom = DateOnly.FromDateTime(_minValidDate),
					ValidTo = DateOnly.FromDateTime(_minValidDate.AddMonths(1)),
					SubscriptionDate = DateTime.Now.AddHours(5).AddMinutes(30),
					UserId = _user?.Id ?? 0,
					Status = true,
				};

				StateHasChanged();

				return;
			}
		}

		_subscription = new SubscriptionModel
		{
			Id = 0,
			PersonId = _person?.Id ?? 0,
			NoSessions = 1,
			Booking = 0,
			SessionTypeId = _sessionTypes.LastOrDefault()?.Id ?? 2,
			ValidFrom = DateOnly.FromDateTime(DateTime.Now),
			ValidTo = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
			SubscriptionDate = DateTime.Now.AddHours(5).AddMinutes(30),
			UserId = _user?.Id ?? 0,
			Status = true,
		};

		_minValidDate = DateTime.Now.AddMonths(-10);

		StateHasChanged();
	}
	#endregion

	#region Saving
	private async Task<bool> ValidateForm()
	{
		if (_person is null || _person.Id <= 0 || string.IsNullOrEmpty(_person.Number))
		{
			await JS.InvokeVoidAsync("alert", "Please enter a valid Person");
			return false;
		}

		_person = await PersonData.LoadPersonByNumber(_person.Number.Trim());
		if (_person is null)
		{
			await JS.InvokeVoidAsync("alert", "Person not found.");
			return false;
		}

		_subscription.PersonId = _person.Id;

		if (_subscription.SessionTypeId <= 0)
		{
			await JS.InvokeVoidAsync("alert", "Please select a valid session type.");
			return false;
		}

		if (_subscription.NoSessions <= 0)
		{
			await JS.InvokeVoidAsync("alert", "Please enter a valid number of sessions.");
			return false;
		}

		if (_subscription.Booking < 0)
		{
			await JS.InvokeVoidAsync("alert", "Please enter a valid booking amount.");
			return false;
		}

		if (_subscription.ValidFrom > _subscription.ValidTo)
		{
			await JS.InvokeVoidAsync("alert", "Start date cannot be greater than end date.");
			return false;
		}

		if (_cash + _card + _upi > _subscription.Booking)
		{
			await JS.InvokeVoidAsync("alert", "Amount Collected is greater than booking Amount.");
			return false;
		}

		// Check for overlapping subscriptions
		if (SubscriptionId is null || SubscriptionId.Value <= 0)
		{
			var existingSubscriptions = await SubscriptionData.LoadSubscriptionByPerson(_person.Id);
			foreach (var subscription in existingSubscriptions)
				// Check if new subscription dates overlap with any existing subscription
				if (_subscription.ValidFrom <= subscription.SubscriptionValidTo && _subscription.ValidTo >= subscription.SubscriptionValidFrom)
				{
					await JS.InvokeVoidAsync("alert",
						$"This person already has a subscription that overlaps with the selected date range " +
						$"(existing subscription: {subscription.SubscriptionValidFrom} to {subscription.SubscriptionValidTo})");
					return false;
				}
		}

		return true;
	}

	private async Task OnSaveClick()
	{
		if (!await ValidateForm())
			return;

		_subscription.Id = await SubscriptionData.InsertSubscription(_subscription);
		if (_subscription.Id == 0)
		{
			await JS.InvokeVoidAsync("alert", "Error in creating subscription.");
			return;
		}

		if (SubscriptionId.HasValue && SubscriptionId.Value > 0)
		{
			var payments = await SubscriptionData.LoadSubscriptionPaymentDetailsBySubscriptionId(SubscriptionId.Value);
			foreach (var payment in payments)
			{
				payment.Status = false;
				await SubscriptionData.InsertSubscriptionPaymentDetails(payment);
			}
		}

		for (int i = 1; i <= 3; i++)
		{
			int amount = i switch
			{
				1 => _cash,
				2 => _card,
				3 => _upi,
				_ => 0
			};

			if (amount == 0)
				continue;

			await SubscriptionData.InsertSubscriptionPaymentDetails(new()
			{
				Id = 0,
				PaymentModeId = i,
				SubscriptionId = _subscription.Id,
				Amount = amount,
				PaymentDate = DateTime.Now,
				Status = true
			});
		}

		if (SubscriptionId.HasValue && SubscriptionId.Value > 0)
			NavManager.NavigateTo("/");

		else
			NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
	}
	#endregion
}