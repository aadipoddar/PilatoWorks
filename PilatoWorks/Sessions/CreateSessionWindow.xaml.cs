using System.Windows;

namespace PilatoWorks.Sessions;

/// <summary>
/// Interaction logic for CreateSessionWindow.xaml
/// </summary>
public partial class CreateSessionWindow : Window
{
	private readonly DateTime _sessionDate;
	private readonly int _slotId;

	public CreateSessionWindow(DateTime sessionDate, int slotId)
	{
		InitializeComponent();

		_sessionDate = sessionDate;
		_slotId = slotId;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		sessionDatePicker.SelectedDate = _sessionDate;

		slotComboBox.ItemsSource = await CommonData.LoadTableData<SlotModel>(TableNames.Slot);
		slotComboBox.DisplayMemberPath = nameof(SlotModel.Hour);
		slotComboBox.SelectedValuePath = nameof(SlotModel.Id);
		slotComboBox.SelectedValue = _slotId;

		personComboBox.ItemsSource = await SubscriptionData.LoadValidSubscriptionByDate(DateOnly.FromDateTime(sessionDatePicker.SelectedDate.Value));
		personComboBox.DisplayMemberPath = nameof(ValidSubscriptionModel.PersonName);
		personComboBox.SelectedValuePath = nameof(ValidSubscriptionModel.PersonId);

		trainer1ComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);
		trainer1ComboBox.DisplayMemberPath = nameof(TrainerModel.Name);
		trainer1ComboBox.SelectedValuePath = nameof(TrainerModel.Id);
		trainer1ComboBox.SelectedIndex = -1;

		trainer2ComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);
		trainer2ComboBox.DisplayMemberPath = nameof(TrainerModel.Name);
		trainer2ComboBox.SelectedValuePath = nameof(TrainerModel.Id);
		trainer2ComboBox.SelectedIndex = -1;
	}

	private async void sessionDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		personComboBox.ItemsSource = await SubscriptionData.LoadValidSubscriptionByDate(DateOnly.FromDateTime(sessionDatePicker.SelectedDate.Value));
		personComboBox.DisplayMemberPath = nameof(ValidSubscriptionModel.PersonName);
		personComboBox.SelectedValuePath = nameof(ValidSubscriptionModel.PersonId);
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (slotComboBox.SelectedValue is null)
		{
			MessageBox.Show("Please select a slot.");
			return;
		}

		if (personComboBox.SelectedValue is null)
		{
			MessageBox.Show("Please select a person.");
			return;
		}

		if (trainer1ComboBox.SelectedValue is null && trainer2ComboBox.SelectedValue is null)
		{
			MessageBox.Show("Please select at least one trainer.");
			return;
		}

		if (trainer1ComboBox.SelectedValue == trainer2ComboBox.SelectedValue)
		{
			MessageBox.Show("Please select different trainers.");
			return;
		}

		await SessionData.InsertSession(new SessionModel
		{
			SessionDate = DateOnly.FromDateTime(sessionDatePicker.SelectedDate.Value),
			SlotId = (int)slotComboBox.SelectedValue,
			PersonId = (int)personComboBox.SelectedValue,
			Trainer1Id = trainer1ComboBox.SelectedValue != null ? (int)trainer1ComboBox.SelectedValue : null,
			Trainer2Id = trainer2ComboBox.SelectedValue != null ? (int)trainer2ComboBox.SelectedValue : null,
			Confirmed = (bool)confirmedCheckBox.IsChecked
		});
		Close();
	}
}
