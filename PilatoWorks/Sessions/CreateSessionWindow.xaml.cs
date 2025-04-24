using System.Windows;

namespace PilatoWorks.Sessions;

/// <summary>
/// Interaction logic for CreateSessionWindow.xaml
/// </summary>
public partial class CreateSessionWindow : Window
{
	private readonly DateOnly _sessionDate;
	private readonly int _slotId;
	private readonly SessionModel _session;
	private readonly SessionsWindow _sessionsWindow;

	public CreateSessionWindow(DateOnly sessionDate, int slotId, SessionsWindow sessionsWindow)
	{
		InitializeComponent();

		_sessionDate = sessionDate;
		_slotId = slotId;
		_sessionsWindow = sessionsWindow;
	}

	public CreateSessionWindow(SessionModel sessionModel, SessionsWindow sessionsWindow)
	{
		InitializeComponent();

		_session = sessionModel;
		_sessionDate = sessionModel.SessionDate;
		_slotId = sessionModel.SlotId;
		_sessionsWindow = sessionsWindow;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		sessionDatePicker.SelectedDate = _sessionDate.ToDateTime(TimeOnly.MinValue);

		await LoadPersonSubscription();

		slotComboBox.ItemsSource = await CommonData.LoadTableData<SlotModel>(TableNames.Slot);
		slotComboBox.DisplayMemberPath = nameof(SlotModel.Hour);
		slotComboBox.SelectedValuePath = nameof(SlotModel.Id);
		slotComboBox.SelectedValue = _slotId;

		trainer1ComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);
		trainer1ComboBox.DisplayMemberPath = nameof(TrainerModel.Name);
		trainer1ComboBox.SelectedValuePath = nameof(TrainerModel.Id);
		trainer1ComboBox.SelectedIndex = 0;

		trainer2ComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);
		trainer2ComboBox.DisplayMemberPath = nameof(TrainerModel.Name);
		trainer2ComboBox.SelectedValuePath = nameof(TrainerModel.Id);
		trainer2ComboBox.SelectedIndex = -1;

		if (_session is not null)
		{
			sessionDatePicker.SelectedDate = _session.SessionDate.ToDateTime(TimeOnly.MinValue);
			slotComboBox.SelectedValue = _session.SlotId;
			trainer1ComboBox.SelectedValue = _session.Trainer1Id;
			trainer2ComboBox.SelectedValue = _session.Trainer2Id;
			confirmedCheckBox.IsChecked = _session.Confirmed;

			personSubscriptionComboBox.Visibility = Visibility.Hidden;
			personSubscriptionTextBox.Visibility = Visibility.Visible;

			var subscription = await CommonData.LoadTableDataById<SubscriptionModel>(TableNames.Subscription, _session.SubscriptionId);
			var person = await CommonData.LoadTableDataById<PersonModel>(TableNames.Person, subscription.PersonId);
			personSubscriptionTextBox.Text = person.Name;

			slotComboBox.IsReadOnly = true;
			sessionDatePicker.Visibility = Visibility.Hidden;
			sessionDateLabel.Visibility = Visibility.Hidden;
		}
	}

	private async void sessionDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) =>
		await LoadPersonSubscription();

	private async void slotComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) =>
		await LoadPersonSubscription();

	private async Task LoadPersonSubscription()
	{
		var validSubs = await SubscriptionData.LoadValidSubscriptionByDate(DateOnly.FromDateTime(sessionDatePicker.SelectedDate.Value));

		var filteredSubs = validSubs
			.GroupBy(sub => sub.PersonId)
			.Select(group => group.First())
			.ToList();


		if (sessionDatePicker.SelectedDate is null || sessionDatePicker is null || slotComboBox is null || slotComboBox.SelectedValue is null) return;

		var slotSessions = await SessionData.LoadSessionDetailsByDateSlot(DateOnly.FromDateTime(sessionDatePicker.SelectedDate.Value), (int)slotComboBox.SelectedValue);
		foreach (var slotSession in slotSessions)
		{
			var subscription = filteredSubs.FirstOrDefault(sub => sub.SubscriptionId == slotSession.SubscriptionId);
			if (subscription is not null)
				filteredSubs.Remove(subscription);
		}

		personSubscriptionComboBox.ItemsSource = filteredSubs;
		personSubscriptionComboBox.DisplayMemberPath = nameof(ValidSubscriptionModel.PersonName);
		personSubscriptionComboBox.SelectedValuePath = nameof(ValidSubscriptionModel.SubscriptionId);

		personSubscriptionComboBox.Items.Refresh();
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		await SessionData.InsertSession(new SessionModel
		{
			Id = _session?.Id ?? 0,
			SessionDate = _session?.SessionDate ?? DateOnly.FromDateTime(sessionDatePicker.SelectedDate.Value),
			SlotId = _session?.SlotId ?? (int)slotComboBox.SelectedValue,
			SubscriptionId = _session?.SubscriptionId ?? (int)personSubscriptionComboBox.SelectedValue,
			Trainer1Id = (int)trainer1ComboBox.SelectedValue,
			Trainer2Id = trainer2ComboBox.SelectedValue is not null ? (int)trainer2ComboBox.SelectedValue : null,
			Confirmed = (bool)confirmedCheckBox.IsChecked
		});

		await _sessionsWindow.LoadSessions();
		Close();
	}

	private bool ValidateForm()
	{
		if (slotComboBox.SelectedValue is null)
		{
			MessageBox.Show("Please select a slot.");
			return false;
		}

		if (personSubscriptionComboBox.SelectedValue is null && personSubscriptionComboBox.Visibility == Visibility.Visible)
		{
			MessageBox.Show("Please select a person.");
			return false;
		}

		if (trainer1ComboBox.SelectedValue == trainer2ComboBox.SelectedValue)
		{
			MessageBox.Show("Please select different trainers.");
			return false;
		}

		return true;
	}
}
