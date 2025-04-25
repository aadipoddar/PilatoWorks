using System.Windows;
using System.Windows.Controls;

namespace PilatoWorks.Subscription;

/// <summary>
/// Interaction logic for SubscriptionPage.xaml
/// </summary>
public partial class SubscriptionPage : Page
{
	private readonly UserModel _user;
	private readonly List<SubscriptionPaymentModeModel> _paymentModels = [];
	private int _subscriptionId = 0;

	public SubscriptionPage(UserModel user)
	{
		InitializeComponent();

		_user = user;
	}

	private async void Page_Loaded(object sender, RoutedEventArgs e)
	{
		validSubscriptionsDataGrid.ItemsSource = await CommonData.LoadTableData<ValidSubscriptionModel>(ViewNames.ValidSubscriptionsDetails);

		subscriptionTypeComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<SessionTypeModel>(TableNames.SessionType);
		subscriptionTypeComboBox.DisplayMemberPath = nameof(SessionTypeModel.Name);
		subscriptionTypeComboBox.SelectedValuePath = nameof(SessionTypeModel.Id);
		subscriptionTypeComboBox.SelectedIndex = 0;

		paymentModeComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<PaymentModeModel>(TableNames.PaymentModes);
		paymentModeComboBox.DisplayMemberPath = nameof(PaymentModeModel.Name);
		paymentModeComboBox.SelectedValuePath = nameof(PaymentModeModel.Id);
		paymentModeComboBox.SelectedIndex = 0;

		amountDataGrid.ItemsSource = _paymentModels;
	}

	private void personNumberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private async void personNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		var foundPerson = await PersonData.LoadPersonByNumber(personNumberTextBox.Text.Trim());

		if (foundPerson is not null)
		{
			personNameTextBox.Text = foundPerson.Name;

			var validSubscriptions = await SubscriptionData.LoadValidSubscriptionByPerson(foundPerson.Id);

			if (validSubscriptions is not null)
			{
				validFromDatePicker.DisplayDateStart = validSubscriptions.SubscriptionValidTo.ToDateTime(new TimeOnly(0, 0));
				validToDatePicker.DisplayDateStart = validSubscriptions.SubscriptionValidTo.ToDateTime(new TimeOnly(0, 0)).AddDays(1);

				if (MessageBox.Show("This person already has a valid subscription. Do you want to update it?", "Update Subscription", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
				{
					_subscriptionId = 0;
					return;
				}

				_subscriptionId = validSubscriptions.SubscriptionId;
				validFromDatePicker.SelectedDate = validSubscriptions.SubscriptionValidFrom.ToDateTime(new TimeOnly(0, 0));
				validToDatePicker.SelectedDate = validSubscriptions.SubscriptionValidTo.ToDateTime(new TimeOnly(0, 0));
				subscriptionTypeComboBox.SelectedValue = validSubscriptions.SessionTypeId;
				noSessionsTextBox.Text = validSubscriptions.NoOfSessions.ToString();
				bookingTextBox.Text = validSubscriptions.BookingAmount.ToString();

				_paymentModels.Clear();

				var subscriptionPayments = await SubscriptionData.LoadSubscriptionPaymentDetailsBySubscriptionId(validSubscriptions.SubscriptionId);

				if (subscriptionPayments is null) return;

				foreach (var payment in subscriptionPayments)
				{
					_paymentModels.Add(new SubscriptionPaymentModeModel
					{
						Id = payment.Id,
						PaymentModeId = payment.PaymentModeId,
						PaymentModeName = (await CommonData.LoadTableDataById<PaymentModeModel>(TableNames.PaymentModes, payment.PaymentModeId)).Name,
						Amount = payment.Amount
					});
				}
			}

			else
			{
				_subscriptionId = 0;

				validFromDatePicker.DisplayDateStart = DateTime.Now.AddDays(-30);
				validToDatePicker.DisplayDateStart = DateTime.Now.AddDays(-30);

				validFromDatePicker.SelectedDate = DateTime.Now;
				validToDatePicker.SelectedDate = DateTime.Now.AddDays(30);

				bookingTextBox.Clear();
				noSessionsTextBox.Clear();
				_paymentModels.Clear();
			}
		}

		else
		{
			_subscriptionId = 0;

			validFromDatePicker.DisplayDateStart = DateTime.Now.AddDays(-30);
			validToDatePicker.DisplayDateStart = DateTime.Now.AddDays(-30);

			validFromDatePicker.SelectedDate = DateTime.Now;
			validToDatePicker.SelectedDate = DateTime.Now.AddDays(30);

			personNameTextBox.Clear();
			bookingTextBox.Clear();
			noSessionsTextBox.Clear();
			_paymentModels.Clear();
		}

		amountDataGrid.Items.Refresh();
	}

	#region DataGridEvents
	private void addButton_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(amountTextBox.Text)) return;
		var selectedPaymentMode = paymentModeComboBox.SelectedItem as PaymentModeModel;

		if (_paymentModels.Any(x => x.PaymentModeId == selectedPaymentMode.Id))
			_paymentModels.FirstOrDefault(x => x.PaymentModeId == selectedPaymentMode.Id).Amount += int.Parse(amountTextBox.Text);

		else _paymentModels.Add(new SubscriptionPaymentModeModel
		{
			Id = 0,
			PaymentModeId = selectedPaymentMode.Id,
			PaymentModeName = selectedPaymentMode.Name,
			Amount = int.Parse(amountTextBox.Text)
		});

		amountTextBox.Text = "0";

		amountDataGrid.Items.Refresh();
	}

	private void amountTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private void amountDataGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (amountDataGrid.SelectedItem is SubscriptionPaymentModeModel selectedPayment)
			_paymentModels.Remove(selectedPayment);

		amountDataGrid.Items.Refresh();
	}
	#endregion

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		var person = await PersonData.LoadPersonByNumber(personNumberTextBox.Text.Trim());
		if (person is null)
		{
			MessageBox.Show("Person not found. Create a New One.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		var subscription = new SubscriptionModel
		{
			Id = _subscriptionId,
			PersonId = person.Id,
			ValidFrom = DateOnly.FromDateTime(validFromDatePicker.SelectedDate.Value),
			ValidTo = DateOnly.FromDateTime(validToDatePicker.SelectedDate.Value),
			SessionTypeId = ((SessionTypeModel)subscriptionTypeComboBox.SelectedItem).Id,
			NoSessions = int.Parse(noSessionsTextBox.Text.Trim()),
			Booking = int.Parse(bookingTextBox.Text.Trim()),
			UserId = _user.Id,
			Status = (bool)statusCheckBox.IsChecked,
			SubscriptionDate = DateTime.Now
		};
		subscription.Id = await SubscriptionData.InsertSubscription(subscription);

		if (subscription.Id == 0)
		{
			MessageBox.Show("Error saving subscription.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		var subscriptionPayments = await SubscriptionData.LoadSubscriptionPaymentDetailsBySubscriptionId(subscription.Id);
		if (subscriptionPayments is not null)
			foreach (var payment in subscriptionPayments)
			{
				payment.Status = false;
				await SubscriptionData.InsertSubscriptionPaymentDetails(payment);
			}

		foreach (var payment in _paymentModels)
			await SubscriptionData.InsertSubscriptionPaymentDetails(new SubscriptionPaymentDetailsModel
			{
				Id = payment.Id,
				SubscriptionId = subscription.Id,
				Amount = payment.Amount,
				PaymentModeId = payment.PaymentModeId,
				PaymentDate = DateTime.Now,
				Status = true
			});

		personNumberTextBox.Clear();

		validSubscriptionsDataGrid.ItemsSource = await CommonData.LoadTableData<ValidSubscriptionModel>(ViewNames.ValidSubscriptionsDetails);
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(personNumberTextBox.Text.Trim()) || string.IsNullOrEmpty(personNameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a valid Person", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(bookingTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a valid booking amount.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (validFromDatePicker.SelectedDate is null)
		{
			MessageBox.Show("Please select a valid start date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (validToDatePicker.SelectedDate is null)
		{
			MessageBox.Show("Please select a valid end date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (validFromDatePicker.SelectedDate > validToDatePicker.SelectedDate)
		{
			MessageBox.Show("Start date cannot be greater than end date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(noSessionsTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a valid number of sessions.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (_paymentModels.Sum(x => x.Amount) > int.Parse(bookingTextBox.Text.Trim()))
		{
			MessageBox.Show("Amount Collected is greater than booking Amount.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private void validSubscriptionsGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (validSubscriptionsDataGrid.SelectedItem is null) return;

		personNumberTextBox.Text = ((ValidSubscriptionModel)validSubscriptionsDataGrid.SelectedItem).PersonNumber;
	}
}
