using System.Windows;

namespace PilatoWorks.Sessions;

/// <summary>
/// Interaction logic for SessionsWindow.xaml
/// </summary>
public partial class SessionsWindow : Window
{
	private readonly UserModel _user;
	private readonly DateTime _selectedDate;
	private readonly SelectDateWindow _selectDateWindow;

	public SessionsWindow(UserModel user, DateTime selectedDate, SelectDateWindow selectDateWindow)
	{
		InitializeComponent();

		_user = user;
		_selectedDate = selectedDate;
		_selectDateWindow = selectDateWindow;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		datePicker.SelectedDate = _selectedDate;

		slotComboBox.ItemsSource = await CommonData.LoadTableData<SlotModel>(TableNames.Slot);
		slotComboBox.DisplayMemberPath = nameof(SlotModel.Hour);
		slotComboBox.SelectedValuePath = nameof(SlotModel.Id);
		slotComboBox.SelectedIndex = 0;
	}

	public async Task LoadSessions()
	{
		if (datePicker.SelectedDate is null || datePicker is null || slotComboBox is null || slotComboBox.SelectedValue is null) return;

		sessionsDataGrid.ItemsSource = await SessionData.LoadSessionDetailsByDateSlot(DateOnly.FromDateTime(datePicker.SelectedDate.Value), (int)slotComboBox.SelectedValue);
	}

	private async void slotComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) =>
		await LoadSessions();

	private async void datePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) =>
		await LoadSessions();

	private async void createSessionWindow_Click(object sender, RoutedEventArgs e)
	{
		var confirmedSessions = (await SessionData.LoadSessionDetailsByDateSlot(DateOnly.FromDateTime(datePicker.SelectedDate.Value), (int)slotComboBox.SelectedValue)).Count;
		var slotMax = (await CommonData.LoadTableDataById<SlotModel>(TableNames.Slot, (int)slotComboBox.SelectedValue)).Hour;
		if (confirmedSessions >= slotMax)
		{
			MessageBox.Show($"The maximum number of sessions for this slot has been reached ({slotMax}).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		CreateSessionWindow createSessionWindow = new(DateOnly.FromDateTime(datePicker.SelectedDate.Value), (int)slotComboBox.SelectedValue, this);
		createSessionWindow.ShowDialog();
	}

	private void sessionsDataGrid_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
	{
		var selectedSession = (SessionDetailsModel)sessionsDataGrid.SelectedItem;
		if (selectedSession is null) return;

		CreateSessionWindow createSessionWindow = new(new SessionModel
		{
			Id = selectedSession.Id,
			SessionDate = selectedSession.SessionDate,
			SlotId = selectedSession.SlotId,
			SubscriptionId = selectedSession.SubscriptionId,
			Trainer1Id = selectedSession.Trainer1Id,
			Trainer2Id = selectedSession.Trainer2Id,
			Confirmed = selectedSession.Confirmed
		}, this);
		createSessionWindow.ShowDialog();

		sessionsDataGrid.SelectedItem = null;
	}
}
