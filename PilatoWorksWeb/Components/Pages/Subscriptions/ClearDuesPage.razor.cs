using Syncfusion.Blazor.Grids;

namespace PilatoWorksWeb.Components.Pages.Subscriptions;

public partial class ClearDuesPage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private string _subscriptionId;
	private string _personNumber;
	private string _personName;
	private string _booking;
	private string _paidCash;
	private string _paidCard;
	private string _paidUPI;
	private string _dues;
	private string _newCash;
	private string _newCard;
	private string _newUPI;

	private List<SubscriptionDetailsModel> _validSubs = [];
	private SfGrid<SubscriptionDetailsModel> _sfGrid;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");

		if (firstRender)
			await LoadGrid();
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

	private async Task LoadGrid()
	{
		var subscriptions = await CommonData.LoadTableData<SubscriptionDetailsModel>(ViewNames.SubscriptionDetails);
		_validSubs = [.. subscriptions.Where(x => x.Dues > 0)];

		await _sfGrid.Refresh();
		StateHasChanged();
	}

	public async Task RowSelectHandler(RowSelectEventArgs<SubscriptionDetailsModel> args)
	{
		var selectedRow = args.Data;

		if (selectedRow is null)
			return;

		var paymentDetails = await SubscriptionData.LoadSubscriptionPaymentDetailsBySubscriptionId(selectedRow.SubscriptionId);

		_subscriptionId = selectedRow.SubscriptionId.ToString();
		_personNumber = selectedRow.PersonNumber;
		_personName = selectedRow.PersonName;
		_booking = selectedRow.BookingAmount.ToString();
		_dues = selectedRow.Dues.ToString();

		_paidCash = paymentDetails
			.Where(x => x.PaymentModeId == 1)
			.Sum(x => x.Amount)
			.ToString();

		_paidCard = paymentDetails
			.Where(x => x.PaymentModeId == 2)
			.Sum(x => x.Amount)
			.ToString();

		_paidUPI = paymentDetails
			.Where(x => x.PaymentModeId == 3)
			.Sum(x => x.Amount)
			.ToString();

		_newCash = "";
		_newCard = "";
		_newUPI = "";

		StateHasChanged();
	}

	private async Task OnSaveClick()
	{
		if (!await ValidateForm())
			return;

		for (int i = 1; i <= 3; i++)
		{
			int amount = int.Parse(i switch
			{
				1 => _newCash,
				2 => _newCard,
				3 => _newUPI,
				_ => "0"
			});

			// Skip insertion if the amount is zero
			if (amount == 0)
				continue;

			await SubscriptionData.InsertSubscriptionPaymentDetails(new SubscriptionPaymentDetailsModel
			{
				Id = 0,
				PaymentModeId = i,
				SubscriptionId = int.Parse(_subscriptionId),
				Amount = amount,
				PaymentDate = DateTime.Now,
				Status = true
			});
		}

		NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
	}

	private async Task<bool> ValidateForm()
	{
		if (string.IsNullOrEmpty(_newCash)) _newCash = "0";
		if (string.IsNullOrEmpty(_newCard)) _newCard = "0";
		if (string.IsNullOrEmpty(_newUPI)) _newUPI = "0";

		if (string.IsNullOrEmpty(_personNumber) || string.IsNullOrEmpty(_personName))
		{
			await JS.InvokeVoidAsync("alert", "Please select a person.");
			return false;
		}

		if (int.Parse(_newCash) == 0 && int.Parse(_newCard) == 0 && int.Parse(_newUPI) == 0)
		{
			await JS.InvokeVoidAsync("alert", "Please enter a valid amount.");
			return false;
		}

		if (string.IsNullOrEmpty(_subscriptionId))
		{
			await JS.InvokeVoidAsync("alert", "Please select a subscription.");
			return false;
		}

		if (int.Parse(_newCash) + int.Parse(_newCard) + int.Parse(_newUPI) < int.Parse(_dues))
		{
			await JS.InvokeVoidAsync("alert", "Total payment must be greater than dues.");
			return false;
		}

		return true;
	}
}