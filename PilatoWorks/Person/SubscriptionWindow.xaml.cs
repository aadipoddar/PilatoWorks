using System.Windows;

namespace PilatoWorks.Person;

/// <summary>
/// Interaction logic for Subscription.xaml
/// </summary>
public partial class SubscriptionWindow : Window
{
	public SubscriptionWindow() => InitializeComponent();

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		subscriptionTypeComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<SessionTypeModel>(TableNames.SessionType);
		subscriptionTypeComboBox.DisplayMemberPath = nameof(SessionTypeModel.Name);
		subscriptionTypeComboBox.SelectedValuePath = nameof(SessionTypeModel.Id);
		subscriptionTypeComboBox.SelectedIndex = 0;

		paymentModeComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<PaymentModeModel>(TableNames.PaymentModes);
		paymentModeComboBox.DisplayMemberPath = nameof(PaymentModeModel.Name);
		paymentModeComboBox.SelectedValuePath = nameof(PaymentModeModel.Id);
		paymentModeComboBox.SelectedIndex = 0;
	}

	private void personNumberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private async void personNumberTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		var foundPerson = await PersonData.LoadPersonByNumber(personNumberTextBox.Text.Trim());

		if (foundPerson is not null)
		{
			personNameTextBox.Text = foundPerson.Name;

			var validSubscriptions = await SubscriptionData.LoadValidSubscriptionByPerson(foundPerson.Id);

			if (validSubscriptions is not null)
			{
				if (MessageBox.Show("This person already has a valid subscription. Do you want to update it?", "Update Subscription", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
					return;

				validFromDatePicker.SelectedDate = validSubscriptions.ValidFrom.ToDateTime(new TimeOnly(0, 0));
				validToDatePicker.SelectedDate = validSubscriptions.ValidTo.ToDateTime(new TimeOnly(0, 0));
				subscriptionTypeComboBox.SelectedValue = validSubscriptions.SessionTypeId;
				noSessionsTextBox.Text = validSubscriptions.NoSessions.ToString();
				amountTextBox.Text = validSubscriptions.Amount.ToString();
				paymentModeComboBox.SelectedValue = validSubscriptions.PaymentModeId;
				statusCheckBox.IsChecked = validSubscriptions.Status;
			}
			else
			{
				validFromDatePicker.SelectedDate = DateTime.Now;
				validToDatePicker.SelectedDate = DateTime.Now.AddDays(30);
				amountTextBox.Clear();
				noSessionsTextBox.Clear();
				statusCheckBox.IsChecked = true;
			}
		}

		else
			personNameTextBox.Clear();
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(personNumberTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a valid person number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (string.IsNullOrEmpty(amountTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a valid amount.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (validFromDatePicker.SelectedDate is null)
		{
			MessageBox.Show("Please select a valid start date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (validToDatePicker.SelectedDate is null)
		{
			MessageBox.Show("Please select a valid end date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (validFromDatePicker.SelectedDate > validToDatePicker.SelectedDate)
		{
			MessageBox.Show("Start date cannot be greater than end date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (string.IsNullOrEmpty(noSessionsTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a valid number of sessions.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		var person = await PersonData.LoadPersonByNumber(personNumberTextBox.Text.Trim());
		if (person is not null)
		{
			var subscription = new SubscriptionModel
			{
				Id = 0,
				PersonId = person.Id,
				ValidFrom = DateOnly.FromDateTime(validFromDatePicker.SelectedDate.Value),
				ValidTo = DateOnly.FromDateTime(validToDatePicker.SelectedDate.Value),
				SessionTypeId = ((SessionTypeModel)subscriptionTypeComboBox.SelectedItem).Id,
				NoSessions = int.Parse(noSessionsTextBox.Text.Trim()),
				PaymentModeId = ((PaymentModeModel)paymentModeComboBox.SelectedItem).Id,
				Amount = int.Parse(amountTextBox.Text.Trim()),
				Status = (bool)statusCheckBox.IsChecked
			};
			await SubscriptionData.InsertSubscription(subscription);
			Close();
		}

		else
			MessageBox.Show("Person not found. Create a New One.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
	}
}
