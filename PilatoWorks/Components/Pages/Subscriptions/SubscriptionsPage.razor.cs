namespace PilatoWorks.Components.Pages.Subscriptions;

public partial class SubscriptionsPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	[Parameter] public int? SubscriptionId { get; set; }

	public DateOnly ValidStartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
	public DateOnly ValidEndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

	public DateTime MinValidDate { get; set; } = DateTime.Now.AddMonths(-10);

	private UserModel _user;

	private string _personNumber;
	private string _noSessions;
	private string _booking;
	private string _cash;
	private string _card;
	private string _upi;

	private DateTime _subscriptionDate = DateTime.Now;
	private bool _status = true;
	private int _selectedPersonId = 0;
	private int _selectedSessionType = 0;

	private List<PersonModel> _personList = [];
	private List<SessionTypeModel> _sessionTypes = [];

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
		_personList = await CommonData.LoadTableData<PersonModel>(TableNames.Person);
		_sessionTypes = await CommonData.LoadTableDataByStatus<SessionTypeModel>(TableNames.SessionType);
		_selectedSessionType = _sessionTypes.FirstOrDefault()?.Id ?? 0;

		if (SubscriptionId.HasValue && SubscriptionId.Value > 0)
			await LoadSubscription();

		StateHasChanged();
	}

	private async Task LoadSubscription()
	{
		var subscription = await CommonData.LoadTableDataById<SubscriptionModel>(TableNames.Subscription, SubscriptionId.Value);
		if (subscription is null)
		{
			await JS.InvokeVoidAsync("alert", "Subscription not found.");
			NavManager.NavigateTo("/");
			return;
		}

		var person = await CommonData.LoadTableDataById<PersonModel>(TableNames.Person, subscription.PersonId);
		if (person is null)
		{
			await JS.InvokeVoidAsync("alert", "Person not found for the subscription.");
			NavManager.NavigateTo("/");
			return;
		}

		_personNumber = person.Number;
		_selectedPersonId = person.Id;
		_noSessions = subscription.NoSessions.ToString();
		_booking = subscription.Booking.ToString();
		_selectedSessionType = subscription.SessionTypeId;
		_status = subscription.Status;
		_subscriptionDate = subscription.SubscriptionDate;
		_selectedSessionType = subscription.SessionTypeId;
		ValidStartDate = subscription.ValidFrom;
		ValidEndDate = subscription.ValidTo;

		var payments = await SubscriptionData.LoadSubscriptionPaymentDetailsBySubscriptionId(subscription.Id);
		if (payments is not null)
		{
			_cash = payments.FirstOrDefault(p => p.PaymentModeId == 1)?.Amount.ToString() ?? "";
			_card = payments.FirstOrDefault(p => p.PaymentModeId == 2)?.Amount.ToString() ?? "";
			_upi = payments.FirstOrDefault(p => p.PaymentModeId == 3)?.Amount.ToString() ?? "";
		}
		else
		{
			_cash = "";
			_card = "";
			_upi = "";
		}

		StateHasChanged();
	}

	private async Task OnNumberChanged(string args)
	{
		_personNumber = args;
		if (string.IsNullOrEmpty(_personNumber)) return;

		var foundPerson = await PersonData.LoadPersonByNumber(_personNumber.Trim());

		if (foundPerson is not null)
		{
			_selectedPersonId = foundPerson.Id;

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
			_selectedPersonId = 0;

			MinValidDate = DateTime.Now.AddMonths(-10);
			ValidStartDate = DateOnly.FromDateTime(DateTime.Now);
			ValidEndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

			_personNumber = "";
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

	private async Task OnPersonSelected(int personId)
	{
		var selectedPerson = _personList.FirstOrDefault(p => p.Id == personId);

		if (selectedPerson is not null)
		{
			_personNumber = selectedPerson.Number;

			await OnNumberChanged(_personNumber);
		}
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

		var subscriptionId = await SubscriptionData.InsertSubscription(new()
		{
			Id = SubscriptionId ?? 0,
			PersonId = person.Id,
			ValidFrom = ValidStartDate,
			ValidTo = ValidEndDate,
			SessionTypeId = _selectedSessionType,
			NoSessions = int.Parse(_noSessions.Trim()),
			Booking = int.Parse(_booking.Trim()),
			UserId = _user.Id,
			Status = _status,
			SubscriptionDate = _subscriptionDate
		});
		if (subscriptionId == 0)
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
			int amount = int.Parse(i switch
			{
				1 => _cash,
				2 => _card,
				3 => _upi,
				_ => "0"
			});

			if (amount == 0)
				continue;

			await SubscriptionData.InsertSubscriptionPaymentDetails(new()
			{
				Id = 0,
				PaymentModeId = i,
				SubscriptionId = subscriptionId,
				Amount = amount,
				PaymentDate = DateTime.Now,
				Status = true
			});
		}

		if (SubscriptionId.HasValue && SubscriptionId.Value > 0)
			NavManager.NavigateTo($"/");

		else
			NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
	}

	private async Task<bool> ValidateForm()
	{
		if (_selectedPersonId == 0)
		{
			await JS.InvokeVoidAsync("alert", "Please enter a valid Person");
			return false;
		}

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
		if (SubscriptionId is null || SubscriptionId.Value <= 0)
		{
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
		}

		return true;
	}

	private void OnCancelClick() =>
		NavManager.NavigateTo("/");
}